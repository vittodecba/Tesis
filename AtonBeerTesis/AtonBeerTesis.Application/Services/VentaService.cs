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
                    SaldoPendiente = saldoPendiente,
                    Subtotal = v.Subtotal > 0 ? v.Subtotal : v.MontoTotal + v.DescuentoMonto,
                    DescuentoMonto = v.DescuentoMonto,
                    DescuentoPorcentaje = v.DescuentoPorcentaje,
                    MotivoDescuento = v.MotivoDescuento,
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
        public async Task<bool> AplicarDescuentoAsync(int id, AplicarDescuentoDto dto)
        {
            var venta = await _ventaRepository.GetByIdAsync(id);
            if (venta is null) return false;

            if (venta.EstadoVenta == EstadoVenta.Pagado)
                throw new Exception("No se puede modificar el descuento de una venta pagada.");

            var totalPagado = await _pagoRepository.GetTotalPagadoByVentaIdAsync(id);
            if (totalPagado > 0)
                throw new Exception("No se puede modificar el descuento porque la venta ya tiene pagos registrados.");

            if (dto.Valor <= 0)
                throw new Exception("El valor del descuento debe ser mayor a 0.");

            var subtotal = venta.Subtotal > 0 ? venta.Subtotal : venta.MontoTotal + venta.DescuentoMonto;
            if (subtotal <= 0)
                throw new Exception("No se puede calcular el descuento porque el subtotal de la venta no es válido.");

            decimal descuentoMonto;
            decimal descuentoPorcentaje;

            var tipoDescuento = dto.TipoDescuento.Trim().ToLower();

            if (tipoDescuento == "porcentaje")
            {
                if (dto.Valor >= 100)
                    throw new Exception("El porcentaje de descuento no puede ser igual o mayor al 100%.");

                descuentoPorcentaje = dto.Valor;
                descuentoMonto = subtotal * (dto.Valor / 100);
            }
            else if (tipoDescuento == "montofijo")
            {
                descuentoMonto = dto.Valor;
                descuentoPorcentaje = (descuentoMonto / subtotal) * 100;
            }
            else
            {
                throw new Exception("Tipo de descuento inválido.");
            }

            if (descuentoMonto >= subtotal)
                throw new Exception("El descuento no puede igualar o superar el subtotal de la venta.");

            venta.Subtotal = subtotal;
            venta.DescuentoMonto = Math.Round(descuentoMonto, 2);
            venta.DescuentoPorcentaje = Math.Round(descuentoPorcentaje, 2);
            venta.MotivoDescuento = string.IsNullOrWhiteSpace(dto.Motivo) ? "Descuento comercial" : dto.Motivo.Trim();
            venta.MontoTotal = Math.Round(subtotal - descuentoMonto, 2);

            await _ventaRepository.UpdateAsync(venta);
            return true;
        }
    }
}
