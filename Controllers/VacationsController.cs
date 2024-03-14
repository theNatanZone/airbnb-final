using AirbnbProj2.DAL;
using AirbnbProj2.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AirbnbProj2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacationsController : ControllerBase
    {
        private readonly UsersService _usersService;
        private readonly FlatsService _flatsService;
        private readonly VacationsService _vacationsService;

        public VacationsController(UsersService usersService, FlatsService flatsService, VacationsService vacationsService)
        {
            _usersService = usersService;
            _flatsService = flatsService;
            _vacationsService = vacationsService;
        }

        // GET: api/Vacations
        [HttpGet]
        public ActionResult<IEnumerable<Vacation>> Get()
        {
            var vacations = _vacationsService.GetVacations();
            return vacations.Count == 0
                ? NotFound("No vacations found")
                : Ok(vacations);
        }

        // GET api/Vacations/{id}
        [HttpGet("{id}")]
        public ActionResult<Vacation> Get(int id)
        {
            var vacation = _vacationsService.GetById(id);
            return vacation == null
                ? NotFound($"Vacation with ID {id} not found")
                : Ok(vacation);
        }

        // GET api/Vacations/user?userId={Email}
        [HttpGet("user-vacations")]
        public ActionResult<IEnumerable<Vacation>> GetByUser([FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Failed: Invalid user Id");

            var vacations = _vacationsService.GetByUserEmail(userId);
            return vacations.Count == 0
                ? NotFound($"No vacations found for user '{userId}'.")
                : Ok(vacations);
        }

        // GET api/Vacations/search/startDate/{startDate}/endDate/{endDate}   
        [HttpGet("search/startDate/{startDate}/endDate/{endDate}")]
        public IActionResult GetByDates(DateTime startDate, DateTime endDate)
        {
            var filteredVacations = _vacationsService.GetByDates(startDate, endDate);
            return filteredVacations.Count == 0
                ? NotFound("No vacations found in the specified dates")
                : Ok(filteredVacations);
        }

        // POST api/Vacations
        [HttpPost]
        public ActionResult Post([FromBody] Vacation vacation)
        {
            try
            {
                _vacationsService.ValidateVacationModel(vacation);

                if (_vacationsService.GetById(vacation.Id) != null)
                    return BadRequest($"Failed: Vacation with ID '{vacation.Id}' already exists and must be unique.");

                if (!_flatsService.GetFlats().Exists(f => f.Id == vacation.FlatId))
                    return NotFound($"Failed: Flat with ID '{vacation.FlatId}' not found");

                if (_usersService.GetUserByEmail(vacation.UserId) == null)
                    return NotFound($"Failed: User Email '{vacation.UserId}' not found ");

                //if (vacation.EndDate <= vacation.StartDate)
                //    return BadRequest("Failed: End-Date should be greater than Start-Date.");

                if (!_vacationsService.IsAvailable(vacation.FlatId, vacation.StartDate, vacation.EndDate))
                    return BadRequest("Failed: The apartment is already rented in the specified dates");

                var result = _vacationsService.InsertVacation(vacation);
                if (!result)
                    return BadRequest("Vacation insertion failed.");

                var newVacation = _vacationsService.GetById(vacation.Id); 
                return Ok(newVacation);
            }
            catch (Exception e)
            {
                return BadRequest($"Vacation insertion failed with an Exception:\n{e.Message}");
            }

        }

        

        // PUT api/<VacationsController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<VacationsController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
