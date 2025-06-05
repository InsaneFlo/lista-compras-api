using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ListaComprasAPI.Models;
using ListaComprasAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ListaComprasAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] Usuario usuario) {
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
                return BadRequest("Email já está em uso.");

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.SenhaHash);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuário registrado com sucesso.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login) {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (usuario == null) return Unauthorized("Usuário não encontrado.");

            bool senhaOk = BCrypt.Net.BCrypt.Verify(login.Senha, usuario.SenhaHash);
            if (!senhaOk) return Unauthorized("Senha inválida.");

            // Gerar token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }

    public class LoginDTO {
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
    }
}