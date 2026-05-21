using AtonBeerTesis.Application.Dtos.STOCK;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IFormatoEnvaseService
    {
        Task<IEnumerable<FormatoEnvaseDto>> ObtenerTodosAsync();
        Task<FormatoEnvaseDto> CrearFormatoAsync(CreateFormatoEnvaseDto dto);
        Task<bool> EliminarFormatoAsync(int id);
        Task AgregarEstiloATodosLosFormatosAsync(string estilo, int? recetaId = null);
    }
}
