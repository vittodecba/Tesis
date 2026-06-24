using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Dto;
using System.Text.RegularExpressions;

namespace AtonBeerTesis.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<UsuarioDto>> GetAllAsync(bool mostrarInactivos)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            if (!mostrarInactivos)
            {
                usuarios = usuarios.Where(u => u.Activo == true).ToList();
            }

            return usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Activo = u.Activo,
                RolId = u.RolId,
                RolNombre = u.Rol != null ? u.Rol.Nombre : "Sin Rol"
            }).ToList();
        }

        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var u = await _usuarioRepository.GetByIdAsync(id);
            if (u == null) return null;

            return new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Activo = u.Activo,
                RolId = u.RolId,
                RolNombre = u.Rol?.Nombre ?? "Sin Rol"
            };
        }

        public async Task<UsuarioDto> CreateAsync(UsuarioCreateDto dto)
        {
            var email = dto.Email.ToLower().Trim();
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$");

            if (!emailRegex.IsMatch(email))
            {
                throw new Exception("El formato del email no es válido.");
            }

            if (email.EndsWith(".con"))
            {
                throw new Exception("El email no puede terminar en '.con'. ¿Quisiste poner '.com'?");
            }

            if (dto.RolId <= 0)
            {
                throw new Exception("Debe seleccionar un rol válido para el usuario.");
            }

            if (dto.Password != dto.ConfirmarPassword)
            {
                throw new Exception("Las contraseñas no coinciden.");
            }

            var existente = await _usuarioRepository.GetByEmailAsync(email);
            if (existente != null)
            {
                throw new Exception("El email ya está registrado en el sistema.");
            }

            var todosLosUsuarios = await _usuarioRepository.GetAllAsync();
            if (todosLosUsuarios.Any(u => u.Contrasena == dto.Password))
            {
                throw new Exception("Esa contraseña ya está siendo usada por otro usuario. Elegí una distinta.");
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = email,
                Contrasena = dto.Password,
                RolId = dto.RolId,
                Activo = true
            };

            await _usuarioRepository.AddAsync(nuevoUsuario);

            return new UsuarioDto
            {
                Id = nuevoUsuario.Id,
                Nombre = nuevoUsuario.Nombre,
                Email = nuevoUsuario.Email
            };
        }

        public async Task UpdateAsync(int id, UsuarioUpdateDto dto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) throw new Exception("Usuario no encontrado");

            var email = dto.Email.ToLower().Trim();
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$");

            if (!emailRegex.IsMatch(email))
            {
                throw new Exception("El formato del email no es válido.");
            }

            if (email.EndsWith(".con"))
            {
                throw new Exception("El email no puede terminar en '.con'. ¿Quisiste poner '.com'?");
            }

            if (dto.RolId <= 0)
            {
                throw new Exception("Debe seleccionar un rol válido.");
            }

            if (usuario.Email != email)
            {
                var emailOcupado = await _usuarioRepository.GetByEmailAsync(email);
                if (emailOcupado != null) throw new Exception("El email ya está en uso.");
            }

            usuario.Nombre = dto.Nombre;
            usuario.Apellido = dto.Apellido;
            usuario.Email = email;
            usuario.RolId = dto.RolId;
            usuario.Activo = dto.Activo;

            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario != null)
            {
                usuario.Activo = !usuario.Activo;
                await _usuarioRepository.UpdateAsync(usuario);
            }
        }

        public async Task EliminarPermanenteAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) throw new Exception("Usuario no encontrado.");
            if (usuario.Activo) throw new Exception("Solo se pueden eliminar permanentemente usuarios inactivos.");
            await _usuarioRepository.EliminarFisicoAsync(id);
        }
    }
}