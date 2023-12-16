using System;
using CodePulse.Data;
using CodePulse.Helper;
using CodePulse.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.Controllers
{
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }

            var user = await _applicationDbContext.users.
                FirstOrDefaultAsync(x => x.Username == userObj.Username && x.Password == userObj.Password);
            if (user == null)
            {
                return NotFound(new { Message = "User Not Found!" });
            }

            return Ok(new
            {
                Message = "Login Successful"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "User";
            userObj.Token = "";
            await _applicationDbContext.users.AddAsync(userObj);
            await _applicationDbContext.SaveChangesAsync();
            return Ok( new{
                Message = "Registration Successful"
            });
        }
    }
}

