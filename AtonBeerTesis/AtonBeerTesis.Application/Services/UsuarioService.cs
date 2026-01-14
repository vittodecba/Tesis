using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Dtos;

namespace AtonBeerTesis.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<UsuarioDto>> GetAllAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            // Convertimos la Entidad (BD) a DTO (Frontend)
            return usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Activo = u.Activo,
                // Si tiene rol, mostramos el nombre. Si no, "Sin Rol"
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
                RolNombre = u.Rol?.Nombre ?? "Sin Rol"
            };
        }

        public async Task<UsuarioDto> CreateAsync(UsuarioCreateDto dto)
        {
            // Validamos que el email no exista ya 
            var existente = await _usuarioRepository.GetByEmailAsync(dto.Email);
            if (existente != null)
            {
                throw new Exception("El email ya está registrado en el sistema.");
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Password = dto.Password,
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

            // Si cambió el email, verificamos que el nuevo no esté ocupado
            if (usuario.Email != dto.Email)
            {
                var emailOcupado = await _usuarioRepository.GetByEmailAsync(dto.Email);
                if (emailOcupado != null) throw new Exception("El email ya está en uso.");
            }

            usuario.Nombre = dto.Nombre;
            usuario.Apellido = dto.Apellido;
            usuario.Email = dto.Email;
            usuario.RolId = dto.RolId;
            usuario.Activo = dto.Activo;

            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task DeleteAsync(int id)
        {
            // Baja Lógica: Invertimos el estado (si es true pasa a false)
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario != null)
            {
                usuario.Activo = !usuario.Activo;
                await _usuarioRepository.UpdateAsync(usuario);
            }
        }
    }
}