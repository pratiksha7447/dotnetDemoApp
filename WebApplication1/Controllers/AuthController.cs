
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EmployeeContext _context;
        private readonly IConfiguration _config;

        public AuthController(EmployeeContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserContext request)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new AppUser { Name = request.Username, Password = hashedPassword, Email = request.Email, Role = "User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User Created");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserContext request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                  throw new UnauthorizedAccessException("Invalid Credentials");

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwOTg3ODk4OTg5OSIsIm5hbWUiOiJQcmF0aWtzaGEgVGhvcmF0IiwiaWF0IjoxNTE2MjM5MDIyfQ.Ijaou_qW93R6hAah_s_kKuXkXG7xURJTHp__6A2EbdU"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("redirect")]
        public IActionResult RedirectToApp2(string token)
        {
            var app2Url = $"http://localhost:62615/display/sso-login?token={token}";
            return Redirect(app2Url);
        }

        [HttpGet("sso-login")]
        public IActionResult SsoLogin([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing.");
            }

            try
            {
                // Validate token
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "App1",
                    ValidAudience = "App2",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwOTg3ODk4OTg5OSIsIm5hbWUiOiJQcmF0aWtzaGEgVGhvcmF0IiwiaWF0IjoxNTE2MjM5MDIyfQ.Ijaou_qW93R6hAah_s_kKuXkXG7xURJTHp__6A2EbdU")) // Same key as App1
                };

                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, validationParameters, out var validatedToken);

                // If validation succeeds
                var app2FrontendUrl = $"http://localhost:62615/display?token={token}"; // Use string interpolation to embed the token
                return Redirect(app2FrontendUrl);
                return Ok(new { Message = "SSO login successful" });

            }
            catch (Exception)
            {
                return Unauthorized("Invalid token.");
            }
        }
    }
}

