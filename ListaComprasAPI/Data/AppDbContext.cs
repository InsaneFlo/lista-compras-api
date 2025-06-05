using ListaComprasAPI.Models;
using Microsoft.EntityFrameworkCore;



namespace ListaComprasAPI.Data {
    public class AppDbContext : DbContext {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Lista> Listas { get; set; } = null!;
        public DbSet<Item> Itens { get; set; } = null!;

        public DbSet<ListaCompartilhada> ListasCompartilhadas { get; set; } = null!;

        //Método para configurar relacionamentos e outras config do EF core.
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            //Configurar a chave primária composta para ListaCompartilhada
            modelBuilder.Entity<ListaCompartilhada>()
                .HasKey(lc => new { lc.UsuarioId, lc.ListaId });

            //Relacionamento ListaCompartilhada -> Usuario
            modelBuilder.Entity<ListaCompartilhada>()
                .HasOne(lc => lc.Usuario)
                .WithMany(u => u.ListasCompartilhadas)
                .HasForeignKey(lc => lc.UsuarioId);

            //Relacionamento ListaCompartilhada -> Lista
            modelBuilder.Entity<ListaCompartilhada>()
                .HasOne(lc => lc.Lista)
                .WithMany(l => l.ListasCompartilhadas)
                .HasForeignKey(lc => lc.ListaId);
        }
    }
}
