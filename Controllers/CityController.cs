using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using GraphQlIntro.Data;
using GraphQlIntro.Model;
using Microsoft.EntityFrameworkCore;

namespace GrapQLIntro.Controllers
{
	/// <summary>
	/// REST controller for Cities
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class CityController : ControllerBase
	{
		private readonly GeoContext _context;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="context">EF context</param>
		public CityController(GeoContext context)
		{
			_context = context;
		}

		/// <summary>
		/// List all cities, also includes Sites
		/// </summary>
		/// <returns>List of Cities</returns>
		[HttpGet]
		public ActionResult<List<City>> GetAll()
		{
			return _context.Cities.Include(c => c.Sites).ToList();
		}

		/// <summary>
		/// Get details for a city
		/// </summary>
		/// <param name="id">City Id</param>
		/// <returns></returns>
		[HttpGet("{id}", Name = "GetCity")]
		public ActionResult<City> GetById(int id)
		{
			City item = _context.Cities.Find(id);
			return item ?? (ActionResult<City>)NotFound();
		}

		/// <summary>
		/// Add a new city
		/// </summary>
		/// <param name="model">City to create</param>
		/// <returns>Details for creates city</returns>
		[HttpPost]
		public IActionResult Create([FromBody] City model)
		{
			_context.Cities.Add(model);
			_context.SaveChanges();
			return CreatedAtRoute("GetCity",new {id = model.Id}, model);
		}

		/// <summary>
		/// Update existing city
		/// </summary>
		/// <param name="id">id of city</param>
		/// <param name="model">city object with new values</param>
		/// <returns>IActionResult</returns>
		[HttpPut("{id}", Name ="UpdateCity")]
		public IActionResult Put(int id, City model)
		{
			var item = _context.Cities.Find(id);
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
		/// Delete a city
		/// </summary>
		/// <param name="id">Id of city to delete</param>
		/// <returns>IActionResult</returns>
		[HttpDelete("{id}", Name = "DeleteCity")]
		public IActionResult Delete(int id)
		{
			var item = _context.Cities.Find(id);
			if (item == null)
			{
				return NotFound();
			}
			_context.Cities.Remove(item);
			_context.SaveChanges();
			return NoContent();
		}
	}
}