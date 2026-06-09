using AtonBeerTesis.Application.Dtos.VENTAS;
using AtonBeerTesis.Application.DTOs;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaDto>> ObtenerTodasAsync();
        Task<bool> PatchAsync(int id, PatchVentaDto dto);
        Task<ReporteVentasDto> ObtenerReporteVentasAsync(DateTime fechaDesde, DateTime fechaHasta);
        Task<byte[]> GenerarPdfReporteVentasAsync(DateTime fechaDesde, DateTime fechaHasta);
    }
}