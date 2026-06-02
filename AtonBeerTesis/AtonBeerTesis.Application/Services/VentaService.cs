using AtonBeerTesis.Application.Dtos.VENTAS;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IPagoRepository _pagoRepository;
        public VentaService(IVentaRepository ventaRepository, IPagoRepository pagoRepository)
        {
            _ventaRepository = ventaRepository;
            _pagoRepository = pagoRepository;
        }

        public async Task<IEnumerable<VentaDto>> ObtenerTodasAsync()
        {
            var ventas = await _ventaRepository.GetAllAsync();
            var resultado = new List<VentaDto>();
            foreach (var v in ventas)
            {
                var totalPagado = await _pagoRepository.GetTotalPagadoByVentaIdAsync(v.Id);
                var saldoPendiente = v.MontoTotal - totalPagado;

                resultado.Add(new VentaDto
                {
                    Id = v.Id,
                    NumeroVenta = v.NumeroVenta,
                    FechaCreacion = v.FechaCreacion,
                    ClienteId = v.ClienteId,
                    ClienteNombre = v.Cliente?.RazonSocial ?? $"Cliente #{v.ClienteId}",
                    PedidoId = v.PedidoId,
                    MontoTotal = v.MontoTotal,
                    EstadoVenta = v.EstadoVenta.ToString(),
                    Plazo = v.Plazo,
                    MetodoPago = v.MetodoPago.ToString(),
                    TotalPagado = totalPagado,
                    SaldoPendiente = saldoPendiente
                });
            }
            return resultado;
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
