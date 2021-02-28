using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using GraphQlIntro.Model;
using GraphQlIntro.Data;
using Microsoft.EntityFrameworkCore;

namespace GraphQLIntro.Controlles
{
	/// <summary>
	/// REST controller for Countries
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class CountryController : ControllerBase
	{
		private readonly GeoContext _context;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="context">EF Context</param>
		public CountryController(GeoContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Get list of all countries, including capital, cities and sites of cities
		/// </summary>
		/// <returns>List of countries</returns>
		[HttpGet]
		public ActionResult<List<Country>> GetAll()
		{
			return _context.Countries
						.Include(c=>c.Cities)
						.ThenInclude(city=>city.Sites)
						.Include(c=>c.Capital)
						.ToList();
		}

		/// <summary>
		/// Get Details of country, including capital, cities and sites of cities
		/// </summary>
		/// <param name="id">Id of country</param>
		/// <returns>Country object</returns>
		[HttpGet("{id}", Name = "GetCountry")]
		public ActionResult<Country> GetById(int id)
		{
			Country item = _context.Countries
						.Include(c=>c.Cities)
						.ThenInclude(city=>city.Sites)
						.Include(c=>c.Capital)
						.FirstOrDefault(c => c.Id == id);
			return item ?? (ActionResult<Country>)NotFound();
		}

		/// <summary>
		/// Add a new country
		/// </summary>
		/// <param name="model">Country to add</param>
		/// <returns>Added country</returns>
		[HttpPost]
		public IActionResult Create([FromBody] Country model)
		{
			_context.Countries.Add(model);
			_context.SaveChanges();
			return CreatedAtRoute("Country", new { id = model.Id }, model);
		}

		/// <summary>
		/// Update existing country
		/// </summary>
		/// <param name="id">Id of country to update</param>
		/// <param name="model">County object with new properties</param>
		/// <returns>IActionResult</returns>
		[HttpPut("{id}", Name = "PutCountry")]
		public IActionResult Put(int id, Country model)
		{
			var item = _context.Countries.Find(id);
			if (item == null)
			{
				return NotFound();
			}
			item.Name = model.Name;
			_context.Update(item);
			_context.SaveChanges();
			return NoContent();
		}

		/// <summary>
		/// Delete existing country
		/// </summary>
		/// <param name="id">Id of country to delete</param>
		/// <returns>IActionResult</returns>
		[HttpDelete("{id}", Name = "DeletCountry")]
		public IActionResult Delete(int id)
		{
			var item = _context.Countries.Find(id);
			if (item == null)
			{
				return NotFound();
			}
			_context.Countries.Remove(item);
			_context.SaveChanges();
			return NoContent();
		}
	}
}