using AtonBeerTesis.Application.Dtos.VENTAS;
using AtonBeerTesis.Application.DTOs;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaDto>> ObtenerTodasAsync();
        Task<bool> PatchAsync(int id, PatchVentaDto dto);
        Task<bool> AplicarDescuentoAsync(int id, AplicarDescuentoDto dto);
        Task<ReporteVentasDto> ObtenerReporteVentasAsync(DateTime fechaDesde, DateTime fechaHasta, string? cliente = null);
        Task<byte[]> GenerarPdfReporteVentasAsync(ReportePdfRequestDto request);
    }
}