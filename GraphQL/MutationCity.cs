using System.Threading.Tasks;
using GraphQlIntro.Data;
using GraphQlIntro.Model;
using System.Linq;
using HotChocolate;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace GraphQlIntro.GraphQL
{
    public partial class Mutation
    {
		/// <summary>
		/// Add New City to Existing Country
		/// </summary>
		/// <param name="context"></param>
		/// <param name="country"></param>
		/// <param name="city"></param>
		/// <param name="eventSender">reference to GraphQL EventSender</param>
		/// <returns></returns>
		public async Task<City> AddCity([Service] GeoContext context, string country, string city, [Service]ITopicEventSender eventSender)
        {
            Country countryItem = context.Countries.Include(c=>c.Cities).FirstOrDefault(c => c.Name == country);

            if (countryItem == null)
              throw new System.Exception("Country not found.");

            City cityitem = countryItem.Cities.FirstOrDefault(c=>c.Name == city);

            if (cityitem != null)
              throw new System.Exception("City already exists.");

            City newCity = new() { Name = city};
            countryItem.Cities.Add(newCity);
            context.SaveChanges();
            await eventSender.SendAsync(
              nameof(Subscription.OnCityAddedAsync),
              newCity.Id).ConfigureAwait(false);
            return newCity;
        }
    }
}