using System.ComponentModel.DataAnnotations;

namespace ListaComprasAPI.Models {
    public class Usuario {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string SenhaHash { get; set; } = null!;

        public List<Lista> Listas { get; set; } = new();

        public List<ListaCompartilhada> ListasCompartilhadas { get; set; } = new();
    }
}