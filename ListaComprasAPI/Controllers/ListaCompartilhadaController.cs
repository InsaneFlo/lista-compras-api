using ListaComprasAPI.Data;
using ListaComprasAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ListaComprasAPI.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ListaCompartilhadaController : ControllerBase {
        private readonly AppDbContext _context;

        public ListaCompartilhadaController(AppDbContext context) {
            _context = context;
        }

        // Model para receber no body da requisição
        public class CompartilharRequest {
            public int ListaId { get; set; }
            public int UsuarioId { get; set; }
        }

        [HttpPost("compartilhar")]
        public async Task<IActionResult> CompartilharLista([FromBody] CompartilharRequest request) {
            var usuarioIdLogado = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if (usuarioIdLogado == 0) return Unauthorized();

            // Verifica se a lista existe e pertence ao usuário logado
            var lista = await _context.Listas.FindAsync(request.ListaId);
            if (lista == null) return NotFound("Lista não encontrada.");

            if (lista.UsuarioId != usuarioIdLogado)
                return Forbid("Você só pode compartilhar suas próprias listas.");

            // Verificar se já está compartilhado com o usuário alvo
            var jaExiste = await _context.ListasCompartilhadas
                .AnyAsync(lc => lc.ListaId == request.ListaId && lc.UsuarioId == request.UsuarioId);

            if (jaExiste)
                return BadRequest("Essa lista já foi compartilhada com esse usuário.");

            var compartilhamento = new ListaCompartilhada {
                ListaId = request.ListaId,
                UsuarioId = request.UsuarioId
            };

            _context.ListasCompartilhadas.Add(compartilhamento);
            await _context.SaveChangesAsync();

            return Ok("Lista compartilhada com sucesso.");
        }

        [HttpGet("recebidas")]
        public async Task<IActionResult> ObterListasCompartilhadas() {
            var usuarioIdLogado = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if (usuarioIdLogado == 0) return Unauthorized();

            var listas = await _context.ListasCompartilhadas
                .Where(lc => lc.UsuarioId == usuarioIdLogado)
                .Include(lc => lc.Lista)
                .Select(lc => new {
                    lc.Lista.Id,
                    lc.Lista.Nome
                })
                .ToListAsync();

            return Ok(listas);
        }

        [HttpDelete] // Remove/descompartilhe uma lista compartilhada.
        public async Task<IActionResult> DescompartilharLista(int listaId) {

            //Pega o id so usuário do token jwt
            var usuarioIdStr = User.FindFirst("id")?.Value;
            if (usuarioIdStr == null || !int.TryParse(usuarioIdStr, out int usuarioId)) {

                return Unauthorized();
            }

            //Busca o compartilhamento da lista para esse usuário.
            var compartilhamento = await _context.ListasCompartilhadas
                .FirstOrDefaultAsync(lc => lc.ListaId == listaId && lc.UsuarioId == usuarioId);

            if (compartilhamento == null) {

                return NotFound("Compartilhamento impossível.");
            }

            //Remove o compartilhamento
            _context.ListasCompartilhadas.Remove(compartilhamento);
            await _context.SaveChangesAsync();

            return Ok("Lista removida com sucesso!");
        }
    }
}
