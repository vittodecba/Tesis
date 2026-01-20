using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Domain.Entities; 
using Microsoft.AspNetCore.Mvc;
using AtonBeerTesis.Domain.Interfaces;


namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase // O BaseController si lo prefieres
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;

        public UsuarioController(IUsuarioService usuarioService, IUsuarioRepository usuarioRepository, ITokenService tokenService)
        {
            _usuarioService = usuarioService;
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
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

                // Si el mail ya existe, devolvemos error 400 (Bad Request)

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
                Contrasena = Dto.ConfirmarPassword,
                RolId = Dto.RolId, // Aquí podrías forzar un 2 si quieres
                Activo = true
            };
            await _usuarioRepository.AddAsync(nuevUsuario);
            return Ok(nuevUsuario);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto Dto)
        {
            
            var usuario = await _usuarioRepository.GetByEmailAsync(Dto.Email);
            
            if (usuario == null || usuario.Contrasena != Dto.Contrasena)
            {
                return Unauthorized("Credenciales invalidas, reintente nuevamente");
            }

            var token = _tokenService.GenerarTokenJWT(usuario);
            
            return Ok(new {
                success = true,
                message = "Inicio de sesión exitoso",
                data = new {
                    token = token,
                    usuario = new {
                        id = usuario.Id,
                        nombre = usuario.Nombre,
                        email = usuario.Email,
                        rolId = usuario.RolId
                    }
                }
            });
        }
    }
}