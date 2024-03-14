using System.Data;
using System.Diagnostics;
using AirbnbProj2.DAL;
using AirbnbProj2.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AirbnbProj2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlatsController : ControllerBase
    {
        private readonly FlatsService _flatsService;

        public FlatsController(FlatsService flatsService)
        {
            _flatsService = flatsService;
        }

        // GET: api/Flats
        [HttpGet]
        public ActionResult<IEnumerable<Flat>> Get()
        {
            var flats = _flatsService.GetFlats();
            return flats.Count == 0
                ? NotFound("No flats found") 
                : Ok(flats);
        }

        // GET api/Flats/{id}
        [HttpGet("{id}")]
        public ActionResult<Flat> Get(int id)
        {
            var flat = _flatsService.GetFlats().FirstOrDefault(x => x.Id == id);
            return flat == null
                ? NotFound($"Flat with ID {id} not found") 
                : Ok(flat);
        }

        // GET: api/Flats/filter?city={CityName}&maxPrice={MaxPrice}
        [HttpGet("filter")]
        public ActionResult<IEnumerable<Flat>> GetByCityAndMaxPrice([FromQuery] string city, [FromQuery] double maxPrice)
        {
            if (string.IsNullOrEmpty(city) || maxPrice <= 0)
                return BadRequest("Failed: Invalid parameters");

            var filteredFlats = _flatsService.GetByCityAndMaxPrice(city, maxPrice);
            return filteredFlats.Count == 0
                ? NotFound($"Sorry, there are no flats in the city '{city}' with max price of {maxPrice}$")
                : Ok(filteredFlats);
        }

        // POST api/Flats
        [HttpPost]
        public IActionResult Post([FromBody] Flat flat)
        {
            if (flat.NumberOfRooms < 0)
                return BadRequest("Failed: Invalid number of rooms");
            if (flat.Price < 0)
                return BadRequest("Failed: Invalid price");

            try
            {
                var result = _flatsService.InsertFlat(flat);
                if (!result) 
                    return BadRequest("Flat insertion failed.");

                var newFlat = _flatsService.GetFlats()[^1]; //returns the last inserted flat from DB 
                return Ok(newFlat);
            }
            catch (Exception e)
            {
                return BadRequest($"Flat insertion failed with an Exception:\n{e.Message}");
            }
        }

        // PUT api/<FlatsController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<FlatsController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
