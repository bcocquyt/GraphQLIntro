using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlIntro.Data;
using GraphQlIntro.Model;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQlIntro.GraphQL
{
    public partial class Query
    {
        /// <summary>
        /// Get list of cities (matching pattern is optional)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        [UseProjection]
        public IQueryable<City> GetCities([Service] GeoContext context, string pattern) => string.IsNullOrEmpty(pattern) ? context.Cities : context.Cities.Where(c=>c.Name.Contains(pattern));

        /// <summary>
        /// Get Details for one city
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public City GetCity([Service] GeoContext context, int id) => context.Cities.Include(c=>c.Sites).FirstOrDefault(x => x.Id == id);
    }
}