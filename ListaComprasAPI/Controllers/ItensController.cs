using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ListaComprasAPI.Data;
using ListaComprasAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ListaComprasAPI.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItensController : ControllerBase {

        // AppDbContext é injetado automaticamente, para acessar o banco.
        private readonly AppDbContext _context;

        public ItensController(AppDbContext context) {
            _context = context;
        }

        [HttpGet] // Retorna todos os itens das listas do usuário logado (criadas ou compartilhadas).
        public async Task<ActionResult<IEnumerable<Item>>> GetItens() {
            var usuarioIdStr = User.FindFirst("id")?.Value;
            if (!int.TryParse(usuarioIdStr, out var usuarioId))
                return Unauthorized("Usuário não identificado.");

            // ID's das listas criadas pelo usuário
            var listasCriadas = await _context.Listas
                .Where(l => l.UsuarioId == usuarioId)
                .Select(l => l.Id)
                .ToListAsync();

            // ID's das listas compartilhadas com o usuário
            var listasCompartilhadas = await _context.ListasCompartilhadas
                .Where(lc => lc.UsuarioId == usuarioId)
                .Select(lc => lc.ListaId)
                .ToListAsync();

            var todasListas = listasCriadas.Union(listasCompartilhadas);

            var itens = await _context.Itens
                .Where(i => todasListas.Contains(i.ListaId))
                .ToListAsync();

            return itens;
        }

        [HttpGet("{id}")] // Retorna um item específico, pelo Id.
        public async Task<ActionResult<Item>> GetItem(int id) {
            var item = await _context.Itens.FindAsync(id);
            if (item == null) return NotFound();

            return item;
        }

        [HttpPost] // Cria um novo item. (Recebendo nome, quantidade, comprado e listaId).
        public async Task<ActionResult<Item>> PostItem(Item item) {
            _context.Itens.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }

        [HttpPut("{id}")] // Atualiza um item com o Id selecionado.
        public async Task<IActionResult> PutItem(int id, Item item) {
            if (id != item.Id) return BadRequest();

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")] // Remove um item com o id selecionado.
        public async Task<IActionResult> DeleteItem(int id) {
            var item = await _context.Itens.FindAsync(id);
            if (item == null) return NotFound();

            _context.Itens.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
