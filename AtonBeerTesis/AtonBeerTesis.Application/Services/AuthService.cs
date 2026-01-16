using System;
using System.Threading.Tasks;
using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;

namespace AtonBeerTesis.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailService _emailService; // <--- NUEVO: El cartero

        // Inyectamos ambos: el Repositorio y el EmailService
        public AuthService(IUsuarioRepository usuarioRepository, IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
        }

        public async Task<bool> Logout()
        {
            return await Task.FromResult(true);
        }

        public async Task RecuperarContrasena(string email)
        {
            // 1. Buscar al usuario
            var usuario = await _usuarioRepository.GetByEmailAsync(email);

            if (usuario == null)
            {
                throw new Exception("El correo electrónico no se encuentra registrado.");
            }

            // 2. Generar el Token
            string token = Guid.NewGuid().ToString();

            // 3. Guardar el token en la BD
            usuario.TokenRecuperacion = token;
            usuario.TokenExpiracion = DateTime.UtcNow.AddMinutes(15);

            await _usuarioRepository.UpdateAsync(usuario);

            // 4. ENVIAR EL EMAIL (La parte nueva) 🚀
            // El usuario ni se entera si esto falla (try-catch opcional por si se cae internet)
            await _emailService.EnviarEmailRecuperacion(email, token);
        }

        public async Task RestablecerContrasena(RestablecerContrasenaDto dto)
        {
            // 1. Buscamos al usuario
            var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);

            if (usuario == null)
            {
                throw new Exception("El usuario no existe.");
            }

            // 2. VALIDAMOS EL TOKEN
            if (usuario.TokenRecuperacion != dto.Token)
            {
                throw new Exception("El token es inválido.");
            }

            if (DateTime.UtcNow > usuario.TokenExpiracion)
            {
                throw new Exception("El token ha expirado.");
            }

            // 3. CAMBIAMOS LA CONTRASEÑA
            usuario.Password = dto.NuevaPassword; // Acá guardamos la nueva clave

            // 4. LIMPIAMOS EL TOKEN (Para que no se pueda usar de nuevo)
            usuario.TokenRecuperacion = null;
            usuario.TokenExpiracion = null;

            // 5. GUARDAMOS EN BASE DE DATOS
            await _usuarioRepository.UpdateAsync(usuario);
        }

    }


}