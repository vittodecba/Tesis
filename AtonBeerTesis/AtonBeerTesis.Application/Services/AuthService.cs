using System;
using System.Threading.Tasks;
using AtonBeerTesis.Application.Dto; // <--- CAMBIADO A SINGULAR
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;

namespace AtonBeerTesis.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailService _emailService;

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
            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            if (usuario == null) throw new Exception("El correo electrónico no se encuentra registrado.");

            string token = Guid.NewGuid().ToString();
            usuario.TokenRecuperacion = token;
            usuario.TokenExpiracion = DateTime.UtcNow.AddMinutes(15);

            await _usuarioRepository.UpdateAsync(usuario);
            await _emailService.EnviarEmailRecuperacion(email, token);
        }

        public async Task RestablecerContrasena(RestablecerContrasenaDto dto)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);
            if (usuario == null) throw new Exception("El usuario no existe.");

            if (usuario.TokenRecuperacion != dto.Token) throw new Exception("El código es inválido.");
            if (DateTime.UtcNow > usuario.TokenExpiracion) throw new Exception("El código ha expirado.");

            // CAMBIO: Aquí se guarda la nueva clave
            usuario.Contrasena = dto.NuevaPassword;
            usuario.TokenRecuperacion = null;
            usuario.TokenExpiracion = null;

            await _usuarioRepository.UpdateAsync(usuario);
        }
    }
}