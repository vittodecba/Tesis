using AtonBeerTesis.Domain.Entidades;
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
    }
}