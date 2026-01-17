using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entidades;
using AtonBeerTesis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Infrastructure.Repositories
{
    public class UsuarioRepository(ApplicationDbContext context) : BaseRepository<Usuario>(context), IUsuarioRepository
    {
        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
           return await _context.usuarios
                .Include(u => u.Rol) // Incluye que rol tiene el usuario registrado
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
