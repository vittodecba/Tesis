using AtonBeerBackend.Models.DTOs;
using AtonBeerTesis.Application.Dtos.VENTAS;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaDto>> ObtenerTodasAsync();
        Task<bool> PatchAsync(int id, PatchVentaDto dto);
        Task<ReporteVentasDto> ObtenerReporteVentasAsync(DateTime fechaDesde, DateTime fechaHasta);
    }
}
