using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtonBeerTesis.Application.Interfaces;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Services
{
    public class AuthService : IAuthService
    {
        public async Task<bool> Logout()
        {
            // TODO: Acá iría la lógica para invalidar el token (ej: lista negra).
            // Por ahora, devolvemos 'true' para simular que cerró bien.
            return await Task.FromResult(true);
        }
    }
}