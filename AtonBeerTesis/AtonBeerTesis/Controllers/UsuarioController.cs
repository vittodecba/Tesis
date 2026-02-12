using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Application.Dto;

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IUsuarioRepository _usuarioRepository; // Para métodos de Auth directos
        private readonly ITokenService _tokenService;

        public UsuarioController(IUsuarioService usuarioService, IUsuarioRepository usuarioRepository, ITokenService tokenService)
        {
            _usuarioService = usuarioService;
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool mostrarInactivos = false)
        {
            var usuarios = await _usuarioService.GetAllAsync(mostrarInactivos);
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioCreateDto dto)
        {
            try
            {
                var nuevoUsuario = await _usuarioService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = nuevoUsuario.Id }, nuevoUsuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // BOTÓN MODIFICAR
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateDto dto)
        {
            try
            {
                await _usuarioService.UpdateAsync(id, dto);
                return Ok(new { message = "Usuario actualizado con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // BOTÓN ACTIVAR/DESACTIVAR (Usa tu lógica de DeleteAsync que hace toggle)
        [HttpPatch("{id}/toggle-activo")]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            try
            {
                // Tu DeleteAsync actual del Service cambia el booleano Activo
                await _usuarioService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // BOTÓN BORRAR (Si decides implementar borrado físico real)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhysical(int id)
        {
            // Aquí podrías llamar a un método del repositorio que haga Remove() 
            // O seguir usando el toggle según tu preferencia de negocio.
            await _usuarioService.DeleteAsync(id);
            return Ok(new { message = "Estado de usuario actualizado" });
        }

        // --- Auth ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto Dto)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(Dto.Email);
            if (usuario == null || usuario.Contrasena != Dto.Contrasena)
                return Unauthorized("Credenciales invalidas");

            var token = _tokenService.GenerarTokenJWT(usuario);
            return Ok(new { success = true, data = new { token, usuario = new { id = usuario.Id, nombre = usuario.Nombre, email = usuario.Email } } });
        }
    }
}