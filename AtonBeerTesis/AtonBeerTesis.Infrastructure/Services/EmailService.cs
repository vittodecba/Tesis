using AtonBeerTesis.Application.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AtonBeerTesis.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task EnviarEmailRecuperacion(string emailDestino, string token)
        {
            // CONFIGURACIÓN (Después la sacamos a un archivo seguro)
            var miCorreo = "santiagobruera1@gmail.com";
            var miPassword = "uksb sjjn rmsy zcgn";

            var clienteSmtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(miCorreo, miPassword),
                EnableSsl = true,
            };

            var mensaje = new MailMessage
            {
                From = new MailAddress(miCorreo, "Soporte AtonBeer"),
                Subject = "Recuperación de Contraseña",
                IsBodyHtml = true,
                Body = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Recuperar Contraseña</h2>
                        <p>Hemos recibido una solicitud para restablecer tu contraseña.</p>
                        <p>Tu código de seguridad es:</p>
                        <h1 style='color: #d32f2f; letter-spacing: 5px;'>{token}</h1>
                        <p>Este código expira en 15 minutos.</p>
                        <hr>
                        <small>Si no fuiste vos, ignorá este mensaje.</small>
                    </div>"
            };

            mensaje.To.Add(emailDestino);

            await clienteSmtp.SendMailAsync(mensaje);
        }
    }
}