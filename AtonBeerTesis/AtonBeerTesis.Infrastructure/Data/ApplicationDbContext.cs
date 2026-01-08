using AtonBeerTesis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Cliente> Clientes => Set<Cliente>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración simple inline (opcional)
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(x => x.IdCliente);

                entity.Property(x => x.RazonSocial)
                      .IsRequired()
                      .HasMaxLength(150);

                modelBuilder.Entity<Cliente>()
     .HasIndex(c => c.Cuit)
     .IsUnique();


                entity.Property(x => x.Ubicacion)
                      .IsRequired()
                      .HasMaxLength(120);

                entity.Property(x => x.Email).HasMaxLength(120);
                entity.Property(x => x.ContactoNombre).HasMaxLength(120);
                entity.Property(x => x.ContactoTelefono).HasMaxLength(40);
                entity.Property(x => x.ContactoEmail).HasMaxLength(120);

                // Enums como int (por defecto EF lo hace así, esto es explícito)
                entity.Property(x => x.Tipocliente).HasConversion<int>();
                entity.Property(x => x.EstadoCliente).HasConversion<int>();
            });
        }
    }
}
