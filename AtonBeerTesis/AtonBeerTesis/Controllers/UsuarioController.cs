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
        public UsuarioController(IUsuarioRepository usuarioRepository, ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
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
            var UsuarioGuardado = await _usuarioRepository.AddAsync(nuevUsuario);
            return Ok(UsuarioGuardado);
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
