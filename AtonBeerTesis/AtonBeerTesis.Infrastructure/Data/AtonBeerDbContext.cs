using AtonBeerTesis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AtonBeerTesis.Infrastructure.Data
{
    public class AtonBeerDbContext : DbContext
    {
        public AtonBeerDbContext(DbContextOptions<AtonBeerDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}