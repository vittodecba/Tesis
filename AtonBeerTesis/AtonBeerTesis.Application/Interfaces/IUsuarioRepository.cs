using AtonBeerTesis.Domain.Entidades;
using Microsoft.IdentityModel.Tokens;//Nuevo, para criptografia
using System.IdentityModel.Tokens.Jwt;//Nuevo, para crear tokens
using System.Security.Claims;//Nuevo, para crear claims, datos dentro del token
using System.Text;//Nuevo, para codificar texto de la contraseña
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> ObtenerPorEmailAsync(string email);
    }
}
