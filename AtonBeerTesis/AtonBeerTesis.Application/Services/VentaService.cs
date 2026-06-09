using AtonBeerTesis.Application.Dtos.VENTAS;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IFacturaRepository _facturaRepository;

        public VentaService(IVentaRepository ventaRepository, IFacturaRepository facturaRepository)
        {
            _ventaRepository = ventaRepository;
            _facturaRepository = facturaRepository;
        }

        public async Task<IEnumerable<VentaDto>> ObtenerTodasAsync()
        {
            var ventas = await _ventaRepository.GetAllAsync();
            var facturasPorVenta = await _facturaRepository.GetFacturaIdsPorVentaAsync();

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
                MetodoPago    = v.MetodoPago.ToString(),
                TieneFactura  = facturasPorVenta.ContainsKey(v.Id),
                FacturaId     = facturasPorVenta.TryGetValue(v.Id, out var fid) ? fid : null
            });
        }

        public async Task<bool> PatchAsync(int id, PatchVentaDto dto)
        {
            var venta = await _ventaRepository.GetByIdAsync(id);
            if (venta is null) return false;

            // BLOQUEO: venta pagada no puede modificarse
            if (venta.EstadoVenta == EstadoVenta.Pagado)
                throw new Exception("La venta ya está pagada y no puede modificarse.");

            if (dto.Plazo is not null)
                venta.Plazo = dto.Plazo.Value;

            if (dto.MetodoPago is not null)
            {
                if (!Enum.TryParse<MetodoPago>(dto.MetodoPago, ignoreCase: true, out var metodo))
                    throw new Exception($"Método de pago inválido: '{dto.MetodoPago}'.");
                venta.MetodoPago = metodo;
            }

            if (dto.EstadoVenta is not null)
            {
                if (!Enum.TryParse<EstadoVenta>(dto.EstadoVenta, ignoreCase: true, out var estado))
                    throw new Exception($"Estado de venta inválido: '{dto.EstadoVenta}'.");
                venta.EstadoVenta = estado;
            }

            await _ventaRepository.UpdateAsync(venta);
            return true;
        }
    }
}
