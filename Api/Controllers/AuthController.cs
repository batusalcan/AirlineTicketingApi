using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AirlineTicketingApi.DTOs;
using AirlineTicketingApi.Data; 

namespace AirlineTicketingApi.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context; 

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }


        /// <summary>
        /// Authenticates a user and generates a JWT Bearer Token.
        /// </summary>
        /// <remarks>
        /// Validates credentials against the database. Use the generated token to authorize other endpoints.
        /// </remarks>
        /// <param name="request">Username and password</param>
        /// <returns>A JWT Token</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto request)
        {
            
            var user = _context.Users.FirstOrDefault(u => 
                u.FullName == request.Username && 
                u.PasswordHash == request.Password);

            
            if (user != null)
            {
                var token = GenerateJwtToken(user.FullName, user.Role);
                return Ok(new { Token = token });
            }

            
            return Unauthorized(new { Message = "Invalid credentials" });
        }

        private string GenerateJwtToken(string username, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}