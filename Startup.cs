using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using GraphQlIntro.Data;

using GraphQlIntro.GraphQL;

namespace GraphQlIntro
{
	/// <summary>
	/// Statup class to configure what is in cluded in this API
	/// </summary>
    public class Startup
    {
        private const string ApplicationInsightsInstrumentationKeyName = "Telemetry:ApplicationInsights:InstrumentationKey";

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration of key/value application properties.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services">Serivce collection</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<GeoContext>();

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.RespectBrowserAcceptHeader = true;

                RestrictToJsonContentType(options);
                AddEnumAsStringRepresentation(options);
            });

            services.AddHealthChecks();
            services.AddHttpCorrelation();

            // services.TryAddSingleton<InMemoryPubSub>();
            // services.TryAddSingleton<ITopicEventSender>(sp =>
            //     sp.GetRequiredService<InMemoryPubSub>());
            // services.TryAddSingleton<ITopicEventReceiver>(sp =>
            //     sp.GetRequiredService<InMemoryPubSub>());

            // GrapQL Infrastructure
			services.AddGraphQLServer()
                    .AddProjections()
                    .AddSubscriptionType<Subscription>()
        			.AddQueryType<Query>()
                    .AddMutationType<Mutation>()
                    .AddInMemorySubscriptions();

#if DEBUG
            var openApiInformation = new OpenApiInfo
            {
                Title = "GraphQlIntro",
                Version = "v1"
            };

            services.AddSwaggerGen(swaggerGenerationOptions =>
            {
                swaggerGenerationOptions.SwaggerDoc("v1", openApiInformation);
                swaggerGenerationOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "GraphQlIntro.Open-Api.xml"));

                swaggerGenerationOptions.OperationFilter<AddHeaderOperationFilter>("X-Transaction-Id", "Transaction ID is used to correlate multiple operation calls. A new transaction ID will be generated if not specified.", false);
                swaggerGenerationOptions.OperationFilter<AddResponseHeadersFilter>();
            });
#endif
        }

        private static void RestrictToJsonContentType(MvcOptions options)
        {
            var allButJsonInputFormatters = options.InputFormatters.Where(formatter => !(formatter is SystemTextJsonInputFormatter));
            foreach (IInputFormatter inputFormatter in allButJsonInputFormatters)
            {
                options.InputFormatters.Remove(inputFormatter);
            }

            // Removing for text/plain, see https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-3.0#special-case-formatters
            options.OutputFormatters.RemoveType<StringOutputFormatter>();
        }

        private static void AddEnumAsStringRepresentation(MvcOptions options)
        {
            var onlyJsonInputFormatters = options.InputFormatters.OfType<SystemTextJsonInputFormatter>();
            foreach (SystemTextJsonInputFormatter inputFormatter in onlyJsonInputFormatters)
            {
                inputFormatter.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }

            var onlyJsonOutputFormatters = options.OutputFormatters.OfType<SystemTextJsonOutputFormatter>();
            foreach (SystemTextJsonOutputFormatter outputFormatter in onlyJsonOutputFormatters)
            {
                outputFormatter.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandling();
            app.UseHttpCorrelation();
            app.UseWebSockets();
            app.UseRouting();
            app.UseRequestTracking(
                options =>
                {
                    // Whether or not the HTTP request body should be included in the request tracking logging emits.
                    // (default: `false`)
                    options.IncludeRequestBody = true;
                });

#if DEBUG
            app.UseSwagger(swaggerOptions => swaggerOptions.RouteTemplate = "api/{documentName}/docs.json");
            app.UseSwaggerUI(swaggerUiOptions =>
            {
                swaggerUiOptions.SwaggerEndpoint("/api/v1/docs.json", "GraphQlIntro");
                swaggerUiOptions.RoutePrefix = "api/docs";
                swaggerUiOptions.DocumentTitle = "GraphQlIntro";
            });
#endif
            app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
                // GrapQL Endpoint
				endpoints.MapGraphQL();
			});

            Log.Logger = CreateLoggerConfiguration(app.ApplicationServices).CreateLogger();
        }

        private LoggerConfiguration CreateLoggerConfiguration(IServiceProvider serviceProvider)
        {
            var instrumentationKey = Configuration.GetValue<string>(ApplicationInsightsInstrumentationKeyName);

            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithVersion()
                .Enrich.WithComponentName("API")
                .Enrich.WithHttpCorrelationInfo(serviceProvider)
                .WriteTo.Console()
                .WriteTo.AzureApplicationInsights(instrumentationKey)
                ;
        }
    }
}
