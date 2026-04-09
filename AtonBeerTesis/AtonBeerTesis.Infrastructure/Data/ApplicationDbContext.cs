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
        public DbSet<RegistroFermentacion> RegistrosFermentacion { get; set; }
        public DbSet<Rol> roles { get; set; }
        public DbSet<unidadMedida> unidadMedida { get; set; }
        public DbSet<TipoInsumo> TiposInsumo { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<HistorialAcceso> historialAccesos { get; set; }
        public DbSet<ProductoPrueba> ProductosPrueba { get; set; }
        public DbSet<MovimientoStock> MovimientosStock { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<RecetaInsumo> RecetaInsumos { get; set; }
        public DbSet<PasosElaboracion> PasosElaboracion { get; set; }
        public DbSet<Fermentador> Fermentadores { get; set; }
        public DbSet<PlanificacionProduccion> PlanificacionProduccion { get; set; }
        public DbSet<Lote> Lotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── MAPEOS DE TABLAS ──────────────────────────────────────────
            modelBuilder.Entity<Insumo>().ToTable("Insumos");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<unidadMedida>().ToTable("unidadMedida");
            modelBuilder.Entity<TipoInsumo>().ToTable("TiposInsumo");
            modelBuilder.Entity<RecetaInsumo>().ToTable("RecetaInsumos");
            modelBuilder.Entity<Receta>().ToTable("Recetas");
            modelBuilder.Entity<Fermentador>().ToTable("Fermentadores");

            modelBuilder.Entity<Cliente>().HasKey(x => x.IdCliente);

            // ── LOTE ──────────────────────────────────────────────────────
            modelBuilder.Entity<Lote>(entity =>
            {
                entity.ToTable("Lotes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.VolumenLitros).HasPrecision(18, 2);

                entity.HasOne(l => l.Receta)
                    .WithMany()
                    .HasForeignKey(l => l.RecetaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Fermentador)
                    .WithMany()
                    .HasForeignKey(l => l.FermentadorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── REGISTRO FERMENTACION ─────────────────────────────────────
            modelBuilder.Entity<RegistroFermentacion>(entity =>
            {
                entity.ToTable("RegistrosFermentacion");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Ph).HasPrecision(4, 2);
                entity.Property(e => e.Densidad).HasPrecision(6, 3);
                entity.Property(e => e.Temperatura).HasPrecision(5, 2);
                entity.Property(e => e.Presion).HasPrecision(5, 2);

                entity.HasOne(e => e.Lote)
                    .WithMany(l => l.RegistrosFermentacion)
                    .HasForeignKey(e => e.LoteId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.LoteId, e.Fecha })
                    .IsUnique()
                    .HasDatabaseName("IX_Lote_FechaRegistro");

                entity.HasIndex(e => new { e.LoteId, e.DiaFermentacion })
                    .IsUnique()
                    .HasDatabaseName("IX_Lote_DiaFermentacion");
            });

            // ── PLANIFICACION PRODUCCION ──────────────────────────────────
            modelBuilder.Entity<PlanificacionProduccion>(entity =>
            {
                entity.ToTable("PlanificacionProduccion");
                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.Lote)
                    .WithMany()
                    .HasForeignKey(p => p.LoteId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ← Fermentador con mayúscula
                entity.HasOne(p => p.Fermentador)
                    .WithMany()
                    .HasForeignKey(p => p.FermentadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.FermentadorId, e.FechaInicio })
                    .IsUnique()
                    .HasDatabaseName("IX_Fermentador_Fecha");
            });

            // ── RELACIONES ────────────────────────────────────────────────
            modelBuilder.Entity<RecetaInsumo>()
                .HasOne(ri => ri.unidadMedida)
                .WithMany()
                .HasForeignKey(ri => ri.unidadMedidaId)
                .OnDelete(DeleteBehavior.NoAction);

            // ── PRECISIONES DECIMALES ─────────────────────────────────────
            modelBuilder.Entity<Insumo>(e => e.Property(i => i.StockActual).HasPrecision(18, 2));
            modelBuilder.Entity<ProductoPrueba>(e => e.Property(p => p.StockActual).HasPrecision(18, 2));
            modelBuilder.Entity<RecetaInsumo>(e => e.Property(ri => ri.Cantidad).HasPrecision(18, 3));

            modelBuilder.Entity<MovimientoStock>(e =>
            {
                e.Property(m => m.Cantidad).HasPrecision(18, 2);
                e.Property(m => m.StockPrevio).HasPrecision(18, 2);
                e.Property(m => m.StockResultante).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.Property(r => r.Descripcion).HasMaxLength(200);
            });
        }
    }
}