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
        private readonly ITokenService _tokenService; // Para poder acceder a las configuraciones del appsettings.json
        private readonly IHistorialAccesoRepository _historialAccesoRepository; //Repositorio para manejar el historial de accesos
        public UsuarioController(IUsuarioRepository usuarioRepository, ITokenService tokenService, IHistorialAccesoRepository historialAccesoRepository)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _historialAccesoRepository = historialAccesoRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetallAsync()
        {
            var usuario = await _usuarioRepository.GetAllAsync();            
            return Ok(usuario);
        }
        [HttpPost("registro")]
        public async Task<IActionResult> PostAsync([FromBody] UsuarioDto Dto)
        {
            var nuevUsuario = new Usuario
            {
                Nombre = Dto.Nombre,
                Apellido = Dto.Apellido,
                Email = Dto.Email,
                Contraseña = Dto.ConfirmarContrasena,
                RolId = Dto.RolId,
                Activo = true
            };
            //Guardo en la base de datos el nuevo usuario usando el repositorio
            await _usuarioRepository.AddAsync(nuevUsuario);
            var UsuarioGuardado = await _usuarioRepository.ObtenerPorEmailAsync(Dto.Email);
            return Ok(new
            {
                Success = true,
                message = "Usuario registrado exitosamente",
                data = new
                {
                    id = UsuarioGuardado.id,
                    Nombre = UsuarioGuardado.Nombre,
                    email = UsuarioGuardado.Email,
                    rol = UsuarioGuardado.Rol != null ? UsuarioGuardado.Rol.NombreRol : "No asignado"
                },
                StatusCode = 200
            });
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto Dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorEmailAsync(Dto.Email);

            // 1. CASO: El usuario no existe o sea el email no esta registrado.
            if (usuario == null)
            {
                await _historialAccesoRepository.Addasync(new HistorialAcceso
                {
                    EmailIntentado = Dto.Email,
                    Exitoso = false,
                    Detalles = "Email no registrado",
                    FechaIntento = DateTime.Now
                });
                return Unauthorized("Email no registrado, reintente nuevamente");
            }

            // 2. CASO: Contraseña incorrecta
            if (usuario.Contraseña != Dto.Contrasena)
            {
                await _historialAccesoRepository.Addasync(new HistorialAcceso
                {
                    UsuarioId = usuario.id,
                    EmailIntentado = Dto.Email,
                    Exitoso = false,
                    Detalles = "Contraseña incorrecta",
                    FechaIntento = DateTime.Now
                });
                return Unauthorized("Contraseña incorrecta, reintente nuevamente");
            }

            // 3. CASO: Login Exitoso
            var token = _tokenService.GenerarTokenJWT(usuario);

            await _historialAccesoRepository.Addasync(new HistorialAcceso
            {
                UsuarioId = usuario.id,
                EmailIntentado = Dto.Email,
                Exitoso = true,
                Detalles = "Login exitoso",
                FechaIntento = DateTime.Now
            });

            // Retorno con la estructura que espera Angular
            return Ok(new
            {
                success = true,
                message = "Inicio de sesión exitoso",
                data = new
                {
                    token = token,
                    usuario = new
                    {
                        id = usuario.id,
                        nombre = usuario.Nombre,
                        apellido = usuario.Apellido,
                        email = usuario.Email,
                        rolId = usuario.RolId
                    }
                }
            });
        }
        [HttpGet("HistorialAcceso")]//Endpoint para obtener el historial de accesos con filtros opcionales
        public async Task<IActionResult> ObtenerHistorialAsync([FromQuery] string? email, [FromQuery] DateTime? fecha, [FromQuery] bool? exito)
        {
            var historial = await _historialAccesoRepository.ObtenerHistorialAsync(email, fecha, exito);
            var resultado = historial.Select(h => new
            {
                h.Id,
                Usuario = h.Usuario != null ? h.Usuario.Nombre : "Desconocido",
                Email = h.EmailIntentado,
                Fecha = h.FechaIntento.ToString("d"),
                Exitoso = h.Exitoso,
                Detalles = h.Detalles
            });
            return Ok(new {Success=true, data = resultado});
        }
       
    }
}
