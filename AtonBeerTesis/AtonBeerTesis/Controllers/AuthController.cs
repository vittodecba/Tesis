using Microsoft.AspNetCore.Mvc;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Application.DTOs; // <--- ESTO FALTABA
using System.Threading.Tasks;
using AtonBeerTesis.Application.Dtos;

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Este es el botón que va a tocar el Frontend: POST api/auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();
            return Ok(new { message = "Sesión cerrada correctamente" });
        }

        // --- NUEVO METODO AGREGADO ---
        [HttpPost("recuperar-contrasena")]
        public async Task<IActionResult> RecuperarContrasena([FromBody] SolicitudRecuperacionDto solicitud)
        {
            // 1. Validar que el email no venga vacío
            if (string.IsNullOrEmpty(solicitud.Email))
            {
                return BadRequest("El email es obligatorio.");
            }

            await _authService.RecuperarContrasena(solicitud.Email);

            return Ok(new { message = "Si el email existe, se enviaron las instrucciones." });
        }

        [HttpPost("restablecer-contrasena")]
        public async Task<IActionResult> RestablecerContrasena([FromBody] RestablecerContrasenaDto dto)
        {
            try
            {
                await _authService.RestablecerContrasena(dto);
                return Ok(new { message = "Contraseña actualizada con éxito. Ya podés iniciar sesión." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}