using System.Collections.Generic;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace GraphQlIntro.Model
{
    /// <summary>
    /// City model
    /// </summary>
    public class City
    {
        /// <summary>
        /// Unique id for city
        /// </summary>
        /// <value></value>
        public int Id { get; set; }
        /// <summary>
        /// Local of English name of City
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// Sites to see in city
        /// </summary>
        /// <value>Collect of Site</value>
        public ICollection<Site> Sites { get; set; }
    }
}