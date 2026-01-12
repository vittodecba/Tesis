using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : BaseController
    {
       private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetallAsync()
        {
            var usuario = await _usuarioRepository.GetAllAsync();            
            return Ok(usuario);
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] UsuarioDto Dto)
        {
            var nuevUsuario = new Usuario
            {
                Nombre = Dto.Nombre,
                Apellido = Dto.Apellido,
                Email = Dto.Email,
                Contraseña = Dto.ConfirmarContraseña,
                RolId = Dto.RolId,
                Activo = true
            };
            //Guardo en la base de datos el nuevo usuario usando el repositorio
            var UsuarioGuardado = await _usuarioRepository.AddAsync(nuevUsuario);
            return Ok(UsuarioGuardado);
        }
    }
}
