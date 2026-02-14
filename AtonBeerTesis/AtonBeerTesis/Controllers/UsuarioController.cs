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
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService; // Agregado

        public UsuarioController(
            IUsuarioService usuarioService,
            IUsuarioRepository usuarioRepository,
            ITokenService tokenService,
            IAuthService authService) // Agregado
        {
            _usuarioService = usuarioService;
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _authService = authService; // Inyectado
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

        [HttpPatch("{id}/toggle-activo")]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            try
            {
                await _usuarioService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhysical(int id)
        {
            await _usuarioService.DeleteAsync(id);
            return Ok(new { message = "Estado de usuario actualizado" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto Dto)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(Dto.Email);

            if (usuario == null || usuario.Contrasena != Dto.Contrasena)
                return Unauthorized("Credenciales invalidas");

            var token = _tokenService.GenerarTokenJWT(usuario);

            return Ok(new
            {
                success = true,
                data = new
                {
                    token,
                    usuario = new
                    {
                        id = usuario.Id,
                        nombre = usuario.Nombre,
                        email = usuario.Email,
                        rolId = usuario.RolId,
                        rolNombre = usuario.Rol?.Nombre ?? "Sin Rol"
                    }
                }
            });
        }

        // --- MÉTODOS DE RECUPERACIÓN AGREGADOS ---

        [HttpPost("recuperar-contrasena")]
        public async Task<IActionResult> Recuperar([FromBody] RecuperarDto dto)
        {
            try
            {
                await _authService.RecuperarContrasena(dto.Email);
                return Ok(new { message = "Email enviado con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("restablecer-contrasena")]
        public async Task<IActionResult> Restablecer([FromBody] RestablecerContrasenaDto dto)
        {
            try
            {
                await _authService.RestablecerContrasena(dto);
                return Ok(new { message = "Contraseña restablecida con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    // DTO pequeño para la recuperación
    public class RecuperarDto
    {
        public string Email { get; set; }
    }
}