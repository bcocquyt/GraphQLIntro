using System;
using System.Linq;
using GraphQlIntro.Data;
using GraphQlIntro.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace GraphQlIntro
{
	/// <summary>
	/// Main Assembly Class
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// Application Entry Point
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public static int Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.CreateLogger();

			try
			{
				using (GeoContext context = new())
				{
					context.Database.EnsureDeleted();
					context.Database.EnsureCreated();

					if (context.Countries.FirstOrDefault(x => x.Name == "Belgium") == null)
					{
						context.CreateCountry("Belgium", "Brussels", new string[] {"Gent", "Brugge", "Kortrijk", "Genk", "Lier", "Liège", "Mons","Charleroi","Namur"});
                        City c = context.Cities.Include(c=>c.Sites).FirstOrDefault(c=>c.Name == "Gent");
                        c.Sites.Add(new Site{Name = "S.M.A.K."});
                        c.Sites.Add(new Site{Name = "S.T.A.M."});
                        c.Sites.Add(new Site{Name = "Gravensteen"});
                        c.Sites.Add(new Site{Name = "Museum van Schone Kunsten"});
                        c.Sites.Add(new Site{Name = "Korenlei"});
                        c.Sites.Add(new Site{Name = "Graslei"});
                        c.Sites.Add(new Site{Name = "Lam Gods"});
                        c.Sites.Add(new Site{Name = "Sint-Baafs Kathedraal"});
                        context.SaveChanges();
                        c = context.Cities.Include(c=>c.Sites).FirstOrDefault(c=>c.Name == "Brussels");
                        c.Sites.Add(new Site{Name = "Museum van Schone Kunsten"});
                        c.Sites.Add(new Site{Name = "Sint-Goedele en Sint-Michielskatheraal"});
                        c.Sites.Add(new Site{Name = "Grote Markt"});
                        c.Sites.Add(new Site{Name = "Atomium"});
                        c.Sites.Add(new Site{Name = "Museum voor Natuurwetenschappen"});
                        context.SaveChanges();
					}
					if (context.Countries.FirstOrDefault(x => x.Name == "France") == null)
					{
						context.CreateCountry("France", "Paris", new string[] {"Lille","Nancy","Reims","Marseille","Tours","Nantes","Poitiers"});
                        City c = context.Cities.Include(c=>c.Sites).FirstOrDefault(c=>c.Name == "Paris");
                        c.Sites.Add(new Site{Name = "Louvre"});
                        c.Sites.Add(new Site{Name = "Tour Eiffel"});
                        c.Sites.Add(new Site{Name = "Cathédrale de Notre Dame"});
                        c.Sites.Add(new Site{Name = "Hôtel des Invalides"});
                        c.Sites.Add(new Site{Name = "Sacré-Cœur"});
                        context.SaveChanges();
					}
					if (context.Countries.FirstOrDefault(x => x.Name == "Germany") == null)
					{
                        context.CreateCountry("Germany", "Berlin", new string[] {"Frankfurt","Trier","München","Koblenz"});
                        City c = context.Cities.Include(c=>c.Sites).FirstOrDefault(c=>c.Name == "Berlin");
                        c.Sites.Add(new Site{Name = "Brandenburger Tor"});
                        c.Sites.Add(new Site{Name = "Berliner Mauer"});
                        c.Sites.Add(new Site{Name = "Checkpoint Charlie"});
                        c.Sites.Add(new Site{Name = "Altes Stadthaus"});
                        c.Sites.Add(new Site{Name = "Altes Museum"});
                        context.SaveChanges();
					}
					if (context.Countries.FirstOrDefault(x => x.Name == "Italy") == null)
					{
                        context.CreateCountry("Italy", "Roma", new string[] {"Milan", "Naples", "Florence", "Venice", "Bologna","Gubbio","Genova"});
                        City c = context.Cities.Include(c=>c.Sites).FirstOrDefault(c=>c.Name == "Roma");
                        c.Sites.Add(new Site{Name = "Quattro Fontane"});
                        c.Sites.Add(new Site{Name = "Colosseo"});
                        c.Sites.Add(new Site{Name = "Amphitheatrum Flavium"});
                        c.Sites.Add(new Site{Name = "Forum Romanum"});
                        c.Sites.Add(new Site{Name = "Arco di Costantino"});
                        context.SaveChanges();
					}
				}

				CreateHostBuilder(args)
					.Build()
					.Run();

				return 0;
			}
			catch (Exception exception)
			{
				Log.Fatal(exception, "Host terminated unexpectedly");
				return 1;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		/// <summary>
		/// Create HostBuilder for API
		/// </summary>
		/// <param name="args">Command line arguments</param>
		/// <returns>IHostBuilder</returns>
		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			IConfiguration configuration = CreateConfiguration(args);
			IHostBuilder webHostBuilder = CreateHostBuilder(args, configuration);

			return webHostBuilder;
		}

		private static IConfiguration CreateConfiguration(string[] args)
		{
			IConfigurationRoot configuration =
				new ConfigurationBuilder()
					.AddCommandLine(args)
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.AddEnvironmentVariables()
					.Build();

			return configuration;
		}

		private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
		{
			string httpEndpointUrl = "http://+:" + configuration["ARCUS_HTTP_PORT"];
			IHostBuilder webHostBuilder =
				Host.CreateDefaultBuilder(args)
					.ConfigureAppConfiguration(configBuilder => configBuilder.AddConfiguration(configuration))
					.ConfigureWebHostDefaults(webBuilder =>
					{
						webBuilder.ConfigureKestrel(kestrelServerOptions => kestrelServerOptions.AddServerHeader = false)
								  .UseUrls(httpEndpointUrl)
								  .UseSerilog()
								  .UseStartup<Startup>();
					});

			return webHostBuilder;
		}
	}
}