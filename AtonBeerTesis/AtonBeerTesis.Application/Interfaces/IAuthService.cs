using AtonBeerTesis.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IAuthService
    {
        // Definimos que existirá un método para cerrar sesión
        Task<bool> Logout();

        Task RecuperarContrasena(string email);

        Task RestablecerContrasena(RestablecerContrasenaDto dto);
    }
}
