using System.Linq;
using GraphQlIntro.Model;
using Microsoft.EntityFrameworkCore;

namespace GraphQlIntro.Data
{
	/// <summary>
	/// EF Context for Geo db using sqlite db
	/// </summary>
	public class GeoContext : DbContext
	{
		/// <summary>
		/// Configuring the database, for sqlite, set data source filename
		/// </summary>
		/// <param name="options"></param>
		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			_ = options.UseSqlite("Data Source=geo.db");
		}

		/// <summary>
		/// Countries table
		/// </summary>
		public DbSet<Country> Countries { get; set; }
		/// <summary>
		/// Cities table
		/// </summary>
		public DbSet<City> Cities { get; set; }
		/// <summary>
		/// Sites for Cities tables
		/// </summary>
		public DbSet<Site> Sites { get; set; }
		/// <summary>
		/// Helper function to create a new country
		/// </summary>
		/// <param name="countryName">Name of Country</param>
		/// <param name="capitalName">Captial of Country</param>
		/// <param name="cities">Cities of Country</param>
		/// <returns>new Country object</returns>
		public Country CreateCountry(string countryName, string capitalName, string[] cities)
		{
			City capital = new() { Name = capitalName };
			Country country = new() { Name = countryName, Capital = capital };
			Countries.Add(country);
			SaveChanges();

			var ct = Countries
			.Include(c => c.Cities)
			.FirstOrDefault(c => c.Id == country.Id);

			if (ct != null)
			{
				ct.Cities.Add(capital);

				foreach (string cityName in cities)
				{
					City c = new() { Name = cityName };
					ct.Cities.Add(c);
				}
				SaveChanges();
			}
			return ct;
		}
	}
}