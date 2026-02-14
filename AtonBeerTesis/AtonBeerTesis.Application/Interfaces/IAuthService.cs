using System.Threading.Tasks;
using AtonBeerTesis.Application.Dto; // <--- Esto es lo que le faltaba

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> Logout();
        Task RecuperarContrasena(string email);
        Task RestablecerContrasena(RestablecerContrasenaDto dto);
    }
}