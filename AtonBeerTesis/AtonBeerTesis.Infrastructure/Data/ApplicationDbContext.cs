using Microsoft.EntityFrameworkCore;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Entidades;
using AtonBeerTesis.Domain;

namespace AtonBeerTesis.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Rol> roles { get; set; }
        public DbSet<unidadMedida> unidadMedida { get; set; }
        public DbSet<TipoInsumo> TiposInsumo { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<HistorialAcceso> historialAccesos { get; set; }
        public DbSet<ProductoPrueba> ProductosPrueba { get; set; }
        public DbSet<MovimientoStock> MovimientosStock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MAPEOS DE TABLAS (Para que coincidan con SQL)
            modelBuilder.Entity<Insumo>().ToTable("Insumos");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<unidadMedida>().ToTable("unidadMedida"); // ESTA FALTABA
            modelBuilder.Entity<TipoInsumo>().ToTable("TiposInsumo");

            modelBuilder.Entity<Cliente>().HasKey(x => x.IdCliente);

            // Precisiones decimales
            modelBuilder.Entity<Insumo>(e => e.Property(i => i.StockActual).HasPrecision(18, 2));
            modelBuilder.Entity<ProductoPrueba>(e => e.Property(p => p.StockActual).HasPrecision(18, 2));
            modelBuilder.Entity<MovimientoStock>(e => {
                e.Property(m => m.Cantidad).HasPrecision(18, 2);
                e.Property(m => m.StockPrevio).HasPrecision(18, 2);
                e.Property(m => m.StockResultante).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Rol>(entity => {
                entity.Property(r => r.Descripcion).HasMaxLength(200);
            });
        }
    }
}