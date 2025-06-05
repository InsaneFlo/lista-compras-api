using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ListaComprasAPI.Data;
using ListaComprasAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using ListaComprasAPI.DTOs;

namespace ListaComprasAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase {

        //Serve para injetar o banco no controller (para consultar e salvar).
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context) {

            _context = context;
        }

        [HttpGet] //Retorna todos os usuários do banco.
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios() {
            return await _context.Usuarios
                .Select(u => new UsuarioDto {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email
                }).ToListAsync();
        }

        [HttpGet("{id}")] //Busca um só usuário pelo id. Else, retorna not found.
        public async Task<ActionResult<Usuario>> GetUsuario(int id) {

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null) {

                return NotFound();
            }

            return usuario;
        }

        [HttpPost] // Cria um novo usuário (enviando JSON com nome e email da requisição).
        
        public async Task<ActionResult<UsuarioDto>> PostUsuario(UsuarioCreateDto dto) {
            var usuario = new Usuario {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha) // Cripto a senha
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, new UsuarioDto {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            });
        }

        [HttpPut("{id}")] //Atualiza um usuário (Se o id da url bate com o do JSON).
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario) {

            if (id != usuario.Id) {

                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try {

                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException) {

                if (!_context.Usuarios.Any(e => e.Id == id)) {

                    return NotFound();
                }
                else {

                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")] // Remove o usuário pelo id selecionado.
        public async Task<IActionResult> DeleteUsuario(int id) {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario ==  null) {

                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("buscar")]
        [Authorize] // Somente os usuários autenticados podem usar
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> BuscarUsuarios(
         [FromQuery] int? id,
         [FromQuery] string? nome,
         [FromQuery] string? email) {
            var query = _context.Usuarios.AsQueryable();

            if (id.HasValue)
                query = query.Where(u => u.Id == id.Value);

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(u => u.Nome.Contains(nome));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(u => u.Email.Contains(email));

            var usuarios = await query
                .Select(u => new UsuarioDto {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email
                })
                .ToListAsync();

            if (usuarios.Count == 0)
                return NotFound("Nenhum usuário encontrado.");

            return Ok(usuarios);
        }
    }
}