namespace ListaComprasAPI.Models {
    public class ListaCompartilhada {

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public int ListaId { get; set; }
        public Lista Lista { get; set; } = null!;
    }
}
