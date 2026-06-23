using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Interfaces;

namespace AtonBeerTesis.Application.Services
{
    public class RolService : IRolService
    {
        private readonly IRolRepository _rolRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public RolService(IRolRepository rolRepository, IUsuarioRepository usuarioRepository)
        {
            _rolRepository = rolRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<RolDto>> GetAll()
        {
            var roles = await _rolRepository.GetAll();
            // Convertimos de Entidad a DTO
            var rolesDto = new List<RolDto>();
            foreach (var rol in roles)
            {
                rolesDto.Add(new RolDto
                {
                    Id = rol.Id,
                    Nombre = rol.Nombre,
                    Descripcion = rol.Descripcion
                });
            }
            return rolesDto;
        }

        public async Task<RolDto> GetById(int id)
        {
            var rol = await _rolRepository.GetById(id);
            if (rol == null) return null;

            return new RolDto
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                Descripcion = rol.Descripcion
            };
        }

        public async Task Create(RolDto rolDto)
        {
            var rol = new Rol
            {
                Nombre = rolDto.Nombre,
                Descripcion = rolDto.Descripcion
            };
            await _rolRepository.Add(rol);
        }

        public async Task Update(int id, RolDto rolDto)
        {
            var rol = await _rolRepository.GetById(id);
            if (rol != null)
            {
                rol.Nombre = rolDto.Nombre;
                rol.Descripcion = rolDto.Descripcion;
                await _rolRepository.Update(rol);
            }
        }

        public async Task Delete(int id)
        {
            var usuariosConRol = await _usuarioRepository.CountAsync(u => u.RolId == id);
            if (usuariosConRol > 0)
                throw new Exception($"No se puede eliminar el rol porque tiene {usuariosConRol} usuario(s) asignado(s).");

            await _rolRepository.Delete(id);
        }
    }
}