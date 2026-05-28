using AtonBeerTesis.Application.Dtos.VENTAS;
using AtonBeerTesis.Application.Interfaces;

namespace AtonBeerTesis.Application.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;

        public VentaService(IVentaRepository ventaRepository)
        {
            _ventaRepository = ventaRepository;
        }

        public async Task<IEnumerable<VentaDto>> ObtenerTodasAsync()
        {
            var ventas = await _ventaRepository.GetAllAsync();
            return ventas.Select(v => new VentaDto
            {
                Id            = v.Id,
                NumeroVenta   = v.NumeroVenta,
                FechaCreacion = v.FechaCreacion,
                ClienteId     = v.ClienteId,
                ClienteNombre = v.Cliente?.RazonSocial ?? $"Cliente #{v.ClienteId}",
                PedidoId      = v.PedidoId,
                MontoTotal    = v.MontoTotal,
                EstadoVenta   = v.EstadoVenta.ToString(),
                Plazo         = v.Plazo,
                MetodoPago    = v.MetodoPago.ToString()
            });
        }
    }
}
