using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IEmailService
    {
        // Solo necesitamos saber a quién (emailDestino) y qué clave mandarle (token)
        Task EnviarEmailRecuperacion(string emailDestino, string token);
    }
}