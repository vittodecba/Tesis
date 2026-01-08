using Microsoft.AspNetCore.Mvc;
using AtonBeerTesis.Application.Interfaces;
using System.Threading.Tasks;

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Acá le pedimos al sistema que nos traiga el servicio que creaste antes
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
    }
}