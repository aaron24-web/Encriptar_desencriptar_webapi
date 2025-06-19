// JaveragesLibrary/Controllers/V1/AuthController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JaveragesLibrary.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace JaveragesLibrary.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous] // Permite el acceso a este endpoint sin necesidad de un token
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO login)
        {
            // --- Lógica de autenticación de ejemplo ---
            // En una aplicación real, validarías el usuario y contraseña contra una base de datos.
            // Por ahora, usamos credenciales fijas.
            if (login.Username == "admin" && login.Password == "password123")
            {
                var token = GenerateJwtToken(login.Username);
                return Ok(new { token });
            }

            return Unauthorized("Credenciales inválidas");
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Los "claims" son piezas de información sobre el usuario que viajan dentro del token.
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1), // El token es válido por 1 hora
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}