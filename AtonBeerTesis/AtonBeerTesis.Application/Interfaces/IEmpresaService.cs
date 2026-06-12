using AtonBeerTesis.Application.Dtos.EMPRESA;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IEmpresaService
    {
        Task<EmpresaDto?> ObtenerAsync();
        Task<bool> ActualizarAsync(ActualizarEmpresaDto dto);
    }
}
