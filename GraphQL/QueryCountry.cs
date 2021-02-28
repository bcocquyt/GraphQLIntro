using System.Linq;
using GraphQlIntro.Data;
using GraphQlIntro.Model;
using HotChocolate;
using HotChocolate.Data;

namespace GraphQlIntro.GraphQL
{
    public partial class Query
    {
        /// <summary>
        /// Get List of countries
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [UseProjection]
        public IQueryable<Country> GetCountries([Service] GeoContext context) => context.Countries;
    }
}