using Microsoft.EntityFrameworkCore;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain;
// Agregamos los usings de ambas ramas
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtonBeerTesis.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Aquí juntamos todas las tablas (DbSets)
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Rol> roles { get; set; }
        public DbSet <TipoInsumo> TiposInsumo{ get; set; }
        //Este es el DbSet de Insumos que falta
        public DbSet <Insumo> Insumos{ get; set; }


        public DbSet<Insumo> Insumos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuración de Clientes (Viene de HEAD) ---
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(x => x.IdCliente);
                entity.Property(x => x.RazonSocial).IsRequired().HasMaxLength(150);
                modelBuilder.Entity<Cliente>().HasIndex(c => c.Cuit).IsUnique();
                entity.Property(x => x.Ubicacion).IsRequired().HasMaxLength(120);
                entity.Property(x => x.Email).HasMaxLength(120);
                entity.Property(x => x.ContactoNombre).HasMaxLength(120);
                entity.Property(x => x.ContactoTelefono).HasMaxLength(40);
                entity.Property(x => x.ContactoEmail).HasMaxLength(120);
                entity.Property(x => x.Tipocliente).HasConversion<int>();
                entity.Property(x => x.EstadoCliente).HasConversion<int>();
            });

            // --- Configuración de Usuarios y Roles (Viene de tu rama) ---
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");

            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Cocinero", Descripcion = "Registra y consulta procesos productivos, recetas, fermentaciones y estado de barriles." },
                new Rol { Id = 2, Nombre = "ResponsablePlanta", Descripcion = "Supervisa la producción y controla el stock de insumos, barriles y latas." },
                new Rol { Id = 3, Nombre = "ResponsablePedidos", Descripcion = "Registra pedidos, controla entregas y actualiza el estado de los pedidos." },
                new Rol { Id = 4, Nombre = "Gerente", Descripcion = "Gestiona clientes y realiza seguimiento de pedidos." },
                new Rol { Id = 5, Nombre = "GerenteMayor", Descripcion = "Consulta ventas y reportes de ventas para análisis y toma de decisiones." }
            );
        }
    }
}