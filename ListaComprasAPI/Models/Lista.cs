using System.ComponentModel.DataAnnotations;

namespace ListaComprasAPI.Models {
    public class Lista {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da lista é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "A lista deve ter entre 2 e 100 caracteres.")]
        public string Nome { get; set; } = null!;

        public List<Item> Items { get; set; } = new();

        public List<ListaCompartilhada> ListasCompartilhadas { get; set; } = new();

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
