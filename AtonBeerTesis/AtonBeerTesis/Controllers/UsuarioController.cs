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
        [HttpPost("login")]//Voy a agregar al login 
        public async Task<IActionResult> PostAsync([FromBody] LoginDto Dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorEmailAsync(Dto.Email);
            //Valido que el usuario exista y que la contraseña sea correcta
            if (usuario == null)
            {
                await _historialAccesoRepository.Addasync(new HistorialAcceso
                {
                    //Aca el usuario no existe, por lo tanto no puedo guardar el UsuarioId
                    EmailIntentado = Dto.Email,
                    Exitoso = false,
                    Detalles = "Email no registrado",
                    FechaIntento = DateTime.Now
                });
                return Unauthorized("Email no registrado, reintente nuevamente");
            }
            if (usuario.Contraseña != Dto.Contraseña)
            {
                await _historialAccesoRepository.Addasync(new HistorialAcceso
                {
                    UsuarioId = usuario.id,//Aca el usuario ya existe, pero la contraseña es incorrecta
                    EmailIntentado = Dto.Email,
                 Exitoso = false,
                 Detalles = "Contraseña incorrecta",
                 FechaIntento= DateTime.Now
                });
                return Unauthorized("Contraseña incorrecta, reintente nuevamente");
            }
            //Genero el token JWT
            var token = _tokenService.GenerarTokenJWT(usuario);
            //Retorno el token al cliente junto con los datos del usuario
            await _historialAccesoRepository.Addasync(new HistorialAcceso
            {
                UsuarioId = usuario.id,//Aca el login fue exitoso, por lo tanto guardo el UsuarioId
                EmailIntentado = Dto.Email,
             Exitoso = true,
             Detalles = "Login exitoso",
             FechaIntento= DateTime.Now
            });
            return Ok(new
            {
                Mensaje = "Inicio de sesión exitoso",
                Token = token,
                Id = usuario.id,
                Nombre = usuario.Nombre,
                RolId = usuario.RolId,
                Email = usuario.Email
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
        [HttpPost("login")]
        public async Task<IActionResult> PostAsync([FromBody] LoginDto Dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorEmailAsync(Dto.Email);
            //Valido que el usuario exista y que la contraseña sea correcta
            if (usuario == null || usuario.Contraseña != Dto.Contrasena)
            {
                return Unauthorized("Credenciales invalidas, reintente nuevamente");
            }
            //Genero el token JWT
            var token = _tokenService.GenerarTokenJWT(usuario);
            //Retorno el token al cliente junto con los datos del usuario
            return Ok(new
            {
                success = true,
                message = "Inicio de sesión exitoso",
                //Esto ayuda a que el 'map' de Angular lo encuentre
                data = new
                {
                    token = token,
                    //Agrupo los datos en un objeto 
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
    }
}
