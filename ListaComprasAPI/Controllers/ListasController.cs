using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ListaComprasAPI.Data;
using ListaComprasAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ListaComprasAPI.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ListasController : ControllerBase {

        // Injeta automaticamente o AppDbContext (para consultar e alterar dados do banco).
        private readonly AppDbContext _context;

        public ListasController(AppDbContext context) {
            _context = context;
        }

        [HttpGet] // Retorna apenas as listas criadas pelo usuário logado ou compartilhadas com ele.
        public async Task<ActionResult<IEnumerable<Lista>>> GetListas() {
            var usuarioId = int.Parse(User.FindFirst("id")?.Value!);

            var listas = await _context.Listas
                .Where(l =>
                    l.UsuarioId == usuarioId || // Listas criadas por ele
                    l.ListasCompartilhadas.Any(c => c.UsuarioId == usuarioId) // Ou listas compartilhadas com ele
                )
                .Include(l => l.Items) // Inclui os itens da lista
                .ToListAsync();

            return listas;
        }

        [HttpGet("{id}")] // Retorna a lista do id selecionado + todos os itens dessa lista.(.include)
        public async Task<ActionResult<Lista>> GetLista(int id) {
            var usuarioId = int.Parse(User.FindFirst("id")?.Value!);

            var lista = await _context.Listas
                .Include(l => l.Items)
                .Include(l => l.ListasCompartilhadas)
                .FirstOrDefaultAsync(l =>
                    l.Id == id &&
                    (l.UsuarioId == usuarioId || l.ListasCompartilhadas.Any(c => c.UsuarioId == usuarioId))
                );

            if (lista == null) return NotFound();

            return lista;
        }

        [HttpPost] // Cria uma nova lista (Enviando JSON com nome e usuarioId.) + uma lista de itens (mas opcional).
        public async Task<ActionResult<Lista>> PostLista(Lista lista) {
            var usuarioId = int.Parse(User.FindFirst("id")?.Value!);
            lista.UsuarioId = usuarioId;

            _context.Listas.Add(lista);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLista), new { id = lista.Id }, lista);
        }

        [HttpPut("{id}")] // Atualiza uma lista. (Id da url deve bater com o objeto JSON, para não dar BadRequest).
        public async Task<IActionResult> PutLista(int id, Lista lista) {
            if (id != lista.Id) return BadRequest();

            var usuarioId = int.Parse(User.FindFirst("id")?.Value!);
            var listaExistente = await _context.Listas.FindAsync(id);

            if (listaExistente == null) return NotFound();
            if (listaExistente.UsuarioId != usuarioId)
                return Forbid("Você não tem permissão para editar essa lista.");

            listaExistente.Nome = lista.Nome;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")] // Remove uma lista com o Id selecionado.
        public async Task<IActionResult> DeleteLista(int id) {
            var usuarioId = int.Parse(User.FindFirst("id")?.Value!);
            var lista = await _context.Listas.FindAsync(id);

            if (lista == null) return NotFound();
            if (lista.UsuarioId != usuarioId)
                return Forbid("Você não tem permissão para excluir essa lista.");

            _context.Listas.Remove(lista);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
