using System.ComponentModel.DataAnnotations;

namespace ListaComprasAPI.Models {
    public class Item {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do item é obrigatório.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O nome do item deve ter entre 1 e 100 caracteres.")]
        public string Nome { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1.")]
        public int Quantidade { get; set; }

        public bool Comprado { get; set; }

        public int ListaId { get; set; }
        public Lista Lista { get; set; } = null!;
    }
}