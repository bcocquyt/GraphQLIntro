using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlIntro.Data;
using GraphQlIntro.Model;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQlIntro.GraphQL
{
    /// <summary>
    /// GraphQL Subscriptions - Notification on mutation execution
    /// </summary>
    public partial class Subscription
    {
		/// <summary>
		/// Subscription to trigger when a new site has been added
		/// </summary>
		/// <param name="siteId">Id of site that was created</param>
		/// <param name="context">EF Context</param>
		/// <returns></returns>
		[Subscribe]
		[Topic]
        public Task<Site> OnSiteAddedAsync([EventMessage]int siteId, [Service]GeoContext context)
		{
			return context.Sites.FirstOrDefaultAsync(s=>s.Id == siteId);
		}

		/// <summary>
		/// Subscription to trigger when a new city has been added
		/// </summary>
		/// <param name="cityId">Id of added city</param>
		/// <param name="context">EF Context</param>
		/// <returns></returns>
		[Subscribe]
		[Topic]
		public Task<City> OnCityAddedAsync([EventMessage]int cityId, [Service]GeoContext context)
		{
			return context.Cities.FirstOrDefaultAsync(c=>c.Id == cityId);
		}

		/// <summary>
		/// Subscription to trigger when a new country has been added
		/// </summary>
		/// <param name="countryId">Id of added country</param>
		/// <param name="context">EF Context</param>
		/// <returns></returns>
		[Subscribe]
		[Topic]
		public Task<Country> OnCountryAddedAsync([EventMessage]int countryId, [Service]GeoContext context)
		{
			return context.Countries.FirstOrDefaultAsync(c=>c.Id == countryId);
		}
    }
}