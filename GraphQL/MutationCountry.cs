using System.Threading.Tasks;
using GraphQlIntro.Data;
using GraphQlIntro.Model;
using System.Linq;
using HotChocolate;
using HotChocolate.Subscriptions;

namespace GraphQlIntro.GraphQL
{
    public partial class Mutation
    {
		/// <summary>
		/// Create new Country, with capital
		/// </summary>
		/// <param name="context"></param>
		/// <param name="country"></param>
		/// <param name="captial"></param>
		/// <param name="eventSender">reference to GraphQL EventSender</param>
		/// <returns></returns>
		public async Task<Country> AddCountry([Service] GeoContext context, string country, string captial, [Service]ITopicEventSender eventSender)
        {
            Country item = context.Countries.FirstOrDefault(c => c.Name == country);

            if (item != null)
                return item;
                //throw new System.Exception("Country already exists.");

            Country newCountry = context.CreateCountry(country, captial, System.Array.Empty<string>());

            await eventSender.SendAsync(
              nameof(Subscription.OnCountryAddedAsync),
              newCountry.Id).ConfigureAwait(false);

            return newCountry;
        }
    }
}