using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GraphQlIntro.Model
{
    /// <summary>
    /// Country model
    /// </summary>
    public class Country
    {
        /// <summary>
        /// unique id for country
        /// </summary>
        /// <value></value>
        public int Id { get; set; }
        /// <summary>
        /// English name of Country
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Captial of Country
        /// </summary>
        public City Capital { get; set; }
        /// <summary>
        /// Some cities in the country
        /// </summary>
        public ICollection<City> Cities { get; set; }
    }
}