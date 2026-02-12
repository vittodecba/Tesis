using Microsoft.EntityFrameworkCore;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using AtonBeerTesis.Domain.Entidades;

namespace AtonBeerTesis.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Rol> roles { get; set; }
        public DbSet<unidadMedida> unidadMedida { get; set; }
        public DbSet <TipoInsumo> TiposInsumo{ get; set; }
        //Este es el DbSet de Insumos que falta
        public DbSet <Insumo> Insumos{ get; set; }
        public DbSet<HistorialAcceso> historialAccesos { get; set; }

        // DEJA SOLO ESTOS DOS (Borra los otros repetidos)
        public DbSet<ProductoPrueba> ProductosPrueba { get; set; }
        public DbSet<MovimientoStock> MovimientosStock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuración de Clientes ---
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(x => x.IdCliente);
                entity.Property(x => x.RazonSocial).IsRequired().HasMaxLength(150);
                entity.HasIndex(c => c.Cuit).IsUnique();
                entity.Property(x => x.Ubicacion).IsRequired().HasMaxLength(120);
                entity.Property(x => x.Tipocliente).HasConversion<int>();
                entity.Property(x => x.EstadoCliente).HasConversion<int>();
            });

            // --- Configuración de Precisión para Stock (IMPORTANTE) ---
            modelBuilder.Entity<ProductoPrueba>(entity =>
            {
                // Configuramos para que soporte, por ejemplo, 18 dígitos y 2 decimales
                entity.Property(p => p.StockActual).HasPrecision(18, 2);
            });

            modelBuilder.Entity<MovimientoStock>(entity =>
            {
                entity.Property(m => m.Cantidad).HasPrecision(18, 2);
                entity.Property(m => m.StockPrevio).HasPrecision(18, 2);
                entity.Property(m => m.StockResultante).HasPrecision(18, 2);
            });

            // --- Configuración de Usuarios y Roles ---
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");

            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Cocinero", Descripcion = "..." },
                new Rol { Id = 2, Nombre = "ResponsablePlanta", Descripcion = "..." },
                new Rol { Id = 3, Nombre = "ResponsablePedidos", Descripcion = "..." },
                new Rol { Id = 4, Nombre = "Gerente", Descripcion = "..." },
                new Rol { Id = 5, Nombre = "GerenteMayor", Descripcion = "..." }
            );
        }
    }
}