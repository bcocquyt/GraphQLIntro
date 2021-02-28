using System.Threading.Tasks;
using GraphQlIntro.Data;
using GraphQlIntro.Model;
using System.Linq;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Subscriptions;

namespace GraphQlIntro.GraphQL
{
    public partial class Mutation
    {
		/// <summary>
		/// Add site to existing city
		/// </summary>
		/// <param name="context"></param>
		/// <param name="cityId"></param>
		/// <param name="site"></param>
		/// <param name="eventSender"></param>
		/// <returns></returns>
		public async Task<Site> AddSite([Service]GeoContext context, int cityId, string site, [Service]ITopicEventSender eventSender)
        {
            City cityItem = context.Cities.Include(c=>c.Sites).FirstOrDefault(c => c.Id == cityId);

            if (cityItem == null)
              throw new System.Exception("City not found.");

            Site siteitem = cityItem.Sites.FirstOrDefault(s=>s.Name == site);

            if (siteitem != null)
              throw new System.Exception("Site already exists.");

            Site newSite = new() { Name = site};
            cityItem.Sites.Add(newSite);
            context.SaveChanges();

            await eventSender.SendAsync(
              nameof(Subscription.OnSiteAddedAsync),
              newSite.Id).ConfigureAwait(false);

            return newSite;
        }
    }
}