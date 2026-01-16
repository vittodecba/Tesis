using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entidades;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;//Nuevo, para criptografia
using System.IdentityModel.Tokens.Jwt;//Nuevo, para crear tokens
using System.Security.Claims;//Nuevo, para crear claims, datos dentro del token
using System.Text;//Nuevo, para codificar texto de la contraseña
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AtonBeerTesis.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }
        public string GenerarTokenJWT(Usuario usuario)
        {
            //1GENERAR LOS CLAIMS
            var claims = new[]
                {
                //Obtengo los claims del usuario, para guardarlos en el token, claims = datos del usuario.
                    new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),//esto quiere decir "subject", el tema del token, en este caso el email
                    new Claim("id", usuario.id.ToString()),
                    new Claim("nombre", usuario.Nombre),
                    new Claim("rolId", usuario.RolId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())// Esto genera un ID unico para cada token de manera que no se repita
                };
            //2 LEER LA LLAVE SECRETA
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:key"])); //Leo la llave secreta desde appsettings.json
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);//Creo las credenciales usando la llave y el algoritmo de encriptacion HMAC SHA256
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"], //Emisor del token
                audience: _config["Jwt:Audience"], //Audiencia del token
                claims: claims, //Los claims que definimos arriba
                expires: DateTime.Now.AddHours(2), //Tiempo de expiracion del token, duracion de 2 horas
                signingCredentials: credenciales //Las credenciales que definimos arriba
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
