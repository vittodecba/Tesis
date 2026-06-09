using AtonBeerTesis.Application.Dtos.FACTURA;

namespace AtonBeerTesis.Application.Interfaces
{
    public interface IFacturaService
    {
        // Genera la factura (comprobante no fiscal) a partir de una venta entregada.
        Task<FacturaDto> GenerarAsync(int ventaId);

        // Devuelve la factura asociada a una venta (o null si todavía no se generó).
        Task<FacturaDto?> ObtenerPorVentaAsync(int ventaId);

        // Devuelve el PDF (bytes + nombre de archivo) de una factura.
        Task<(byte[] Contenido, string NombreArchivo)?> ObtenerPdfAsync(int facturaId);
    }
}
