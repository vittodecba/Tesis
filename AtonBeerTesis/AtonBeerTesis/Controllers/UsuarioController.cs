using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Domain.Entities; 
using Microsoft.AspNetCore.Mvc;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Domain.Entidades;


namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly ITokenService _tokenService;
        private readonly IHistorialAccesoRepository _historialAccesoRepository;
        public UsuarioController(IUsuarioRepository usuarioRepository, ITokenService tokenService, IHistorialAccesoRepository historialAccesoRepository, IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _historialAccesoRepository = historialAccesoRepository;
        }
        // --- Metodos Santi (Gestión) ---
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

        // --- Metodos Valen (Auth) ---

        [HttpPost("registro")]
        public async Task<IActionResult> PostAsync([FromBody] UsuarioCreateDto Dto)
        {
            var nuevUsuario = new Usuario
            {
                Nombre = Dto.Nombre,
                Apellido = Dto.Apellido,
                Email = Dto.Email,
                Contrasena = Dto.ConfirmarPassword, // Corregido a Contrasena (sin Ñ)
                RolId = Dto.RolId,
                Activo = true
            };

            // Guardo en la base de datos
            await _usuarioRepository.AddAsync(nuevUsuario);
            
            // CORREGIDO: Usamos GetByEmailAsync (Nombre nuevo del repositorio)
            var UsuarioGuardado = await _usuarioRepository.GetByEmailAsync(Dto.Email);

            return Ok(new
            {
                Success = true,
                message = "Usuario registrado exitosamente",
                data = new
                {
                    id = UsuarioGuardado.Id, // Corregido: Id con mayúscula
                    Nombre = UsuarioGuardado.Nombre,
                    email = UsuarioGuardado.Email,
                    rol = UsuarioGuardado.Rol != null ? UsuarioGuardado.Rol.Nombre : "No asignado" // Corregido: Rol.Nombre (verificar si es NombreRol o Nombre en la entidad Rol)
                },
                StatusCode = 200
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto Dto)
        {
            // CORREGIDO: Usamos GetByEmailAsync
            var usuario = await _usuarioRepository.GetByEmailAsync(Dto.Email);

            // 1. CASO: El usuario no existe
            if (usuario == null)
            {
                await _historialAccesoRepository.AddAsync(new HistorialAcceso // Corregido: AddAsync (Mayúscula)
                {
                    EmailIntentado = Dto.Email,
                    Exitoso = false,
                    Detalles = "Email no registrado",
                    FechaIntento = DateTime.Now
                });
                return Unauthorized("Email no registrado, reintente nuevamente");
            }

            // 2. CASO: Contraseña incorrecta
            // Corregido: Contrasena (sin Ñ, porque daba quilombo en el front)
            if (usuario.Contrasena != Dto.Contrasena) 
            {
                await _historialAccesoRepository.AddAsync(new HistorialAcceso
                {
                    UsuarioId = usuario.Id, // Corregido: Id mayúscula
                    EmailIntentado = Dto.Email,
                    Exitoso = false,
                    Detalles = "Contraseña incorrecta",
                    FechaIntento = DateTime.Now
                });
                return Unauthorized("Contraseña incorrecta, reintente nuevamente");
            }

            // 3. CASO: Login Exitoso
            var token = _tokenService.GenerarTokenJWT(usuario);

            await _historialAccesoRepository.AddAsync(new HistorialAcceso
            {
                UsuarioId = usuario.Id,
                EmailIntentado = Dto.Email,
                Exitoso = true,
                Detalles = "Login exitoso",
                FechaIntento = DateTime.Now
            });

            return Ok(new
            {
                success = true,
                message = "Inicio de sesión exitoso",
                data = new
                {
                    token = token,
                    usuario = new
                    {
                        id = usuario.Id,
                        nombre = usuario.Nombre,
                        email = usuario.Email,
                        rolId = usuario.RolId
                    }
                }
            });
        }

        [HttpGet("HistorialAcceso")]
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
            return Ok(new { Success = true, data = resultado });
        }
    }
}