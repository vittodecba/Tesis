using Microsoft.AspNetCore.Mvc;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Application.Dto;
using AtonBeerTesis.Domain.Entidades; // <--- Unificado en singular

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IHistorialAccesoRepository _historialAccesoRepository;

        public AuthController(IAuthService authService, IUsuarioRepository usuarioRepository, ITokenService tokenService, IHistorialAccesoRepository historialAccesoRepository)
        {
            _authService = authService;
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _historialAccesoRepository = historialAccesoRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto Dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var usuario = await _usuarioRepository.GetByEmailAsync(Dto.Email);

            if (usuario == null || usuario.Contrasena != Dto.Contrasena)
            {
                // --- REGISTRO DE INTENTO FALLIDO ---
                var historialFallido = new HistorialAcceso
                {
                    UsuarioId = usuario?.Id, // Si el usuario no existe, será null
                    EmailIntentado = Dto.Email,
                    FechaIntento = DateTime.Now,
                    Exitoso = false,
                    Detalles = usuario == null ? "Email no encontrado" : "Contraseña incorrecta"
                };
                await _historialAccesoRepository.AddAsync(historialFallido);                

                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            // 2. SI LLEGÓ AQUÍ, EL LOGIN ES EXITOSO
            var token = _tokenService.GenerarTokenJWT(usuario);

            // --- REGISTRO DE INTENTO EXITOSO ---
            var historialExitoso = new HistorialAcceso
            {
                UsuarioId = usuario.Id,
                EmailIntentado = usuario.Email,
                FechaIntento = DateTime.Now,
                Exitoso = true,
                Detalles = "Inicio de sesión exitoso desde el sistema"
            };
            await _historialAccesoRepository.AddAsync(historialExitoso);
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

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();
            return Ok(new { message = "Sesión cerrada correctamente" });
        }

        [HttpPost("recuperar-contrasena")]
        public async Task<IActionResult> RecuperarContrasena([FromBody] SolicitudRecuperacionDto solicitud)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _authService.RecuperarContrasena(solicitud.Email);
                return Ok(new { message = "Si el email existe, se enviaron las instrucciones." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("restablecer-contrasena")]
        public async Task<IActionResult> RestablecerContrasena([FromBody] RestablecerContrasenaDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { message = errores });
            }

            try
            {
                await _authService.RestablecerContrasena(dto);
                return Ok(new { message = "Contraseña actualizada con éxito." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}