using AtonBeerTesis.Application.Dtos.Pagos;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtonBeerTesis.Application.Services
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;
        private readonly IVentaRepository _ventaRepository;
        public PagoService(IPagoRepository pagoRepository, IVentaRepository ventaRepository)
        {
            _pagoRepository = pagoRepository;
            _ventaRepository = ventaRepository;
        }
        public async Task<PagosDto> RegistrarPagoAsync(RegistrarPagoDto dto)
        {
            if (dto.Monto <= 0)
            {
                throw new Exception("El monto del pago debe ser mayor a 0.");
            }
            if (dto.Fecha.Date > DateTime.Now.Date)
            {
                throw new Exception("No se puede registrar un pago con fecha futura.");
            }
            var venta = _ventaRepository.GetByIdAsync(dto.VentaId).Result;
            if (venta == null)
            {
                throw new Exception($"No se encontró la venta con ID {dto.VentaId}.");
            }
            if (venta.EstadoVenta == EstadoVenta.Pagado)
            {
                throw new Exception("La venta ya se encuentra pagada.");
            }

            MetodoPago metodo;
            if (dto.MetodoPago == "Efectivo")
            {
                metodo = MetodoPago.Efectivo;
            }
            else if (dto.MetodoPago == "Transferencia")
            {
                metodo = MetodoPago.Transferencia;
            }
            else { throw new Exception($"Método de pago inválido: '{dto.MetodoPago}'."); }

            var totalPagado = await _pagoRepository.GetTotalPagadoByVentaIdAsync(dto.VentaId);
            var saldoPendiente = venta.MontoTotal - totalPagado;

            if (dto.Monto > saldoPendiente)
            {
                throw new Exception($"El pago no puede superar el saldo pendiente. Saldo pendiente: {saldoPendiente}.");
            }
            var pago = new Pago
            {
                VentaId = dto.VentaId,
                Monto = dto.Monto,
                Fecha = dto.Fecha,
                MetodoPago = metodo
            };
            await _pagoRepository.AddAsync(pago);

            var nuevoSaldo = saldoPendiente - dto.Monto;

            if (nuevoSaldo <= 0)
            {
                venta.EstadoVenta = EstadoVenta.Pagado;
            }
            else
            {
                venta.EstadoVenta = EstadoVenta.Pendiente;
            }

            await _ventaRepository.UpdateAsync(venta);

            return new PagosDto
            {
                Id = pago.Id,
                VentaId = pago.VentaId,
                Monto = pago.Monto,
                Fecha = pago.Fecha,
                MetodoPago = pago.MetodoPago.ToString()
            };
        }
        public async Task<IEnumerable<PagosDto>> ObtenerPorVentaAsync(int ventaId)
        {
            var pagos = await _pagoRepository.GetByVentaIdAsync(ventaId);

            return pagos.Select(p => new PagosDto
            {
                Id = p.Id,
                VentaId = p.VentaId,
                Monto = p.Monto,
                Fecha = p.Fecha,
                MetodoPago = p.MetodoPago.ToString()
            });
        }    
    }
}
