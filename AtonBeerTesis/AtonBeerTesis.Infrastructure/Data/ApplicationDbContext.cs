using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Application.Dtos;

namespace AtonBeerTesis.Infrastructure.Data
{
    //El db context es la clase principal que se encarga de la comunicacion con la base de datos
    public class ApplicationDbContext : DbContext
    {
        //Cada DbSet representa una tabla en la base de datos
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Rol> roles { get; set; }
        //Esto es el constructor que recibe las opciones de configuracion para el DbContext
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        //Constructor vacio
        protected ApplicationDbContext()
        {
        }
        //Este metodo se utiliza para configurar las opciones del DbContext
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        //Este metodo se genera justo antes de dar inicio a la BD, a diferencia del OnConfiguring que se ejecuta antes de eso.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Esto dice explicitamente que la entidad StudentsEntity se mapea a la tabla Students
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            base.OnModelCreating(modelBuilder);
            //Aca hago para que el rol se cargue con datos en la BD
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
