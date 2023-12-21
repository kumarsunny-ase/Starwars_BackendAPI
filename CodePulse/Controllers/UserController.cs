using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using CodePulse.Data;
using CodePulse.Helper;
using CodePulse.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
                FirstOrDefaultAsync(x => x.Username == userObj.Username);

            if (user == null)
            {
                return NotFound(new { Message = "User Not Found!" });
            }

            if(!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new { message = "Password is Incorrect" });
            }

            user.Token = CreateJwt(user);
            return Ok(new
            {
                Token = user.Token,
                Message = "Login Successful"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();
            //Check Existed Username
            if (await UserNameExisted(userObj.Username))
                return BadRequest(new { Message = "Username already Exist!" });

            //Check Existed Email
            if (await EmailExisted(userObj.Email))
                return BadRequest(new { Message = "Email already Exist!" });

            // Check Password Strength
            var pass = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString() });

            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "User";
            userObj.Token = "";
            await _applicationDbContext.users.AddAsync(userObj);
            await _applicationDbContext.SaveChangesAsync();
            return Ok( new{
                Message = "Registration Successful"
            });
        }

        private async Task<bool> UserNameExisted(string userName)
        {
            return await _applicationDbContext.users.AnyAsync(x => x.Username == userName);
        }

        private async Task<bool> EmailExisted(string email)
        {
            return await _applicationDbContext.users.AnyAsync(x => x.Email == email);
        }

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 6)
                sb.Append("Minimum password length should be 6" + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]")
                && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be Alphanumeric" + Environment.NewLine);
            if (!Regex.IsMatch(password, "[<,>, @, !,$,%,&,(,),+, *,',#,=]"))
                sb.Append("Password should contain special chars" + Environment.NewLine);

            return sb.ToString();

        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token); 
        }

        [HttpGet("GetAlUser")]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _applicationDbContext.users.ToListAsync());
        }
    }
}

