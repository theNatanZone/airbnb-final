using System;
using System.Data.SqlClient;
using System.Text;
using AirbnbProj2.DAL;
using AirbnbProj2.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AirbnbProj2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;
        private readonly DBService _dbService;

        public UsersController(UsersService usersService, DBService dbService)
        {
            _usersService = usersService;
            _dbService = dbService;
        }

        // POST api/Users/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserCredentials credentials)
        {
            var authenticatedUser = _usersService.GetAuthenticatedUser(credentials);
            if (authenticatedUser == null)
                return Unauthorized("Authentication Failed: Invalid email or password.");
            
            if (!authenticatedUser.IsActive)
                return BadRequest("Login Failed: User is inactive");

            return Ok(UserDto.MapToDto(authenticatedUser));
        }

        // POST api/Users/register
        [HttpPost("register")]
        public IActionResult Post([FromBody] User user)
        {
            try
            {
                _usersService.ValidateUserModel(user);
                if (_usersService.GetUserByEmail(user.Email) != null)
                    return BadRequest("Failed: User with the same email already exists");

                var result = _usersService.InsertUser(user);
                if (!result)
                    return BadRequest("User registration was failed");

                return Ok(UserDto.MapToDto(user));
            }
            catch (Exception e)
            {
                return BadRequest($"User registration failed with an Exception:\n{e.Message}");
            }
        }

        // PUT api/Users/update-details 
        [HttpPut("update-details")]
        public IActionResult Put([FromBody] User user)
        {
            try
            {
                var existingUser = _usersService.GetUserByEmail(user.Email);
                if (existingUser == null)
                    return NotFound($"Failed: User email '{user.Email}' does not exist");

                if (string.IsNullOrEmpty(user.Password)) // if the user does not update his existing password
                {
                    user.Password = existingUser.Password;
                }
                _usersService.ValidateUserModel(user);
                user.IsAdmin = existingUser.IsAdmin;
                user.IsActive = existingUser.IsActive;

                var result = _usersService.UpdateUser(user);
                if (!result)
                    return BadRequest("Failed to update user details");

                return Ok(UserDto.MapToDto(user));
            }
            catch (Exception e)
            {
                return BadRequest($"User update failed with an Exception:\n{e.Message}");
            }
        }

        // GET api/Users/manage-users
        // only authorized admins can get all users 
        [HttpGet("manage-users")]
        public IActionResult GetUsers()
        {
            try
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                {
                    return BadRequest("Authorization header is missing");
                }

                string authToken = Request.Headers["Authorization"];
                if (!authToken.StartsWith("Basic "))
                {
                    return BadRequest("Invalid authentication format");
                }

                // Extract the username and password from the Authorization header
                var base64Credentials = authToken.Substring("Basic ".Length).Trim();
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials));
                var parts = credentials.Split(':', 2);
                var email = parts[0];
                var password = parts[1];
                var userCredentials = new UserCredentials(email, password);

                var user = _usersService.GetAuthenticatedUser(userCredentials);
                bool isAuthenticated = user != null;

                if (!isAuthenticated)
                {
                    return Unauthorized("Admin Authentication Failed: Invalid email or password.");
                }

                if (user is { IsAdmin: false } && user.Email != "admin@gmail.com")
                {
                    return BadRequest("Only admins can manage users");
                }
                var users = _usersService.GetUsers();
                return users.Count == 0
                    ? NotFound("No users found")
                    : Ok(users);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest($"User fetch failed with an Exception:\n{e.Message}");
            }
        }

        [HttpPut("manage-users/activate")]
        public IActionResult ActivateUser([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Failed: Invalid user email");
                }
                var existingUser = _usersService.GetUserByEmail(email);
                if (existingUser == null)
                    return NotFound($"Failed: User email '{email}' does not exist");

                _usersService.CheckEmail(email);

                var result = _usersService.ActivateUser(email);
                if (!result)
                    return BadRequest("Failed to activate user");
                return Ok($"Succeeded: User '{email}' has successfully activated");
            }
            catch (Exception e)
            {
                return BadRequest($"User activating failed with an Exception:\n{e.Message}");
            }
        }

        [HttpPut("manage-users/deactivate")]
        public IActionResult DeactivateUser([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Failed: Invalid user email");
                }
                var existingUser = _usersService.GetUserByEmail(email);
                if (existingUser == null)
                    return NotFound($"Failed: User email '{email}' does not exist");

                _usersService.CheckEmail(email);

                var result = _usersService.DeactivateUser(email);
                if (!result)
                    return BadRequest("Failed to deactivate user");
                return Ok($"Succeeded: User '{email}' has successfully deactivated");
            }
            catch (Exception e)
            {
                return BadRequest($"User deactivating failed with an Exception:\n{e.Message}");
            }
        }

        // GET: api/Users/reports/month?month={Month}
        [HttpGet("reports/month")]
        public IActionResult GetVacationsReportByMonth([FromQuery] int month)
        {
            try
            {
                if (month <= 0 || month > 12)
                {
                    return BadRequest("Invalid month");
                }

                var results = _dbService.GetAverageReport(month);
                
                return results.Count == 0
                    ? NotFound($"No results found for month '{month}'.")
                    : Ok(results);
            }
            catch (Exception e)
            {
                return BadRequest($"Vacation report fetch failed with an Exception:\n{e.Message}");
            }
            
        }

        // DELETE api/<UsersController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

    }

}

