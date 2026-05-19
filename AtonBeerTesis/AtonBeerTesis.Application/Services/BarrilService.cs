using AtonBeerTesis.Application.Dtos.BARRIL;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Application.Services
{
    public interface IBarrilService
    {
        Task<List<BarrilDto>> GetAllAsync();
        Task<BarrilDto> CreateAsync(CreateBarrilDto dto);
        Task<bool> UpdateAsync(int id, UpdateBarrilDto dto);
        Task<bool> EliminarAsync(int id);
        Task<BarrilDetalleDto?> GetDetalleAsync(int id);
        Task<bool> RegistrarMovimientoAsync(RegistrarMovimientoDto dto);
        Task<bool> EliminarUltimoMovimientoAsync(int barrilId);
    }

    public class BarrilService : IBarrilService
    {
        private readonly IBarrilRepository _repository;

        public BarrilService(IBarrilRepository repository)
        {
            _repository = repository;
        }

        private static readonly Dictionary<EstadoBarril, EstadoBarril[]> TransicionesValidas = new()
        {
            [EstadoBarril.Disponible] = [EstadoBarril.Lleno, EstadoBarril.Mantenimiento],
            [EstadoBarril.Lleno] = [EstadoBarril.ConCliente],
            [EstadoBarril.ConCliente] = [EstadoBarril.Sucio],
            [EstadoBarril.Sucio] = [EstadoBarril.EnLavado],
            [EstadoBarril.EnLavado] = [EstadoBarril.Disponible],
            [EstadoBarril.Mantenimiento] = [EstadoBarril.Disponible],
        };

        public async Task<List<BarrilDto>> GetAllAsync()
        {
            var barriles = await _repository.GetAllAsync();
            return barriles.Select(MapToDto).ToList();
        }

        public async Task<BarrilDto> CreateAsync(CreateBarrilDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo))
                throw new Exception("El código del barril es obligatorio.");

            if (await _repository.ExisteCodigoAsync(dto.Codigo))
                throw new Exception($"Ya existe un barril con el código '{dto.Codigo}'.");

            var barril = new Barril
            {
                Codigo = dto.Codigo.Trim().ToUpper(),
                FormatoEnvaseId = dto.FormatoEnvaseId,
                Estado = EstadoBarril.Disponible,
                FechaAdquisicion = dto.FechaAdquisicion,
                UltimaActualizacion = DateTime.Now,
                Observaciones = dto.Observaciones,
            };

            await _repository.AddAsync(barril);
            var creado = await _repository.GetByIdAsync(barril.Id);
            return MapToDto(creado!);
        }

        public async Task<bool> UpdateAsync(int id, UpdateBarrilDto dto)
        {
            var barril = await _repository.GetByIdAsync(id);
            if (barril == null) return false;

            if (dto.Estado.HasValue)
            {
                var nuevoEstado = (EstadoBarril)dto.Estado.Value;
                if (!TransicionesValidas.TryGetValue(barril.Estado, out var permitidos) ||
                    !permitidos.Contains(nuevoEstado))
                {
                    throw new Exception(
                        $"No se puede cambiar el estado de '{ObtenerTextoEstado(barril.Estado)}' " +
                        $"a '{ObtenerTextoEstado(nuevoEstado)}'.");
                }
                barril.Estado = nuevoEstado;
            }

            if (dto.DesasociarCliente)
                barril.ClienteId = null;
            else if (dto.ClienteId.HasValue)
                barril.ClienteId = dto.ClienteId.Value;

            if (dto.FechaAdquisicion.HasValue)
                barril.FechaAdquisicion = dto.FechaAdquisicion.Value;

            barril.Observaciones = dto.Observaciones;
            barril.UltimaActualizacion = DateTime.Now;

            return await _repository.UpdateAsync(barril);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var barril = await _repository.GetByIdAsync(id);
            if (barril == null) return false;

            if (barril.Estado == EstadoBarril.ConCliente)
                throw new Exception($"No se puede eliminar el barril '{barril.Codigo}': está actualmente con un cliente.");

            if (barril.Estado == EstadoBarril.Lleno)
                throw new Exception($"No se puede eliminar el barril '{barril.Codigo}': está lleno y cuenta en el stock. Primero registrá su egreso.");

            return await _repository.EliminarAsync(id);
        }

        public async Task<BarrilDetalleDto?> GetDetalleAsync(int id)
        {
            var barril = await _repository.ObtenerDetalleAsync(id);

            if (barril == null) return null;

            return new BarrilDetalleDto
            {
                Id = barril.Id,
                Codigo = barril.Codigo,
                Formato = barril.FormatoEnvase?.Nombre ?? "N/A",
                Capacidad = (double)(barril.FormatoEnvase?.CapacidadLitros ?? 0m),
                Estado = ObtenerTextoEstado(barril.Estado),
                UbicacionActual = barril.Cliente?.RazonSocial ?? "Fábrica",
                Observaciones = barril.Observaciones,
                LoteId = barril.LoteActualId,
                Estilo = barril.LoteActual?.Receta?.Estilo,
                Receta = barril.LoteActual?.Receta?.Nombre,
                Movimientos = barril.Movimientos.Select(m => new MovimientoItemDto
                {
                    Fecha = m.Fecha,
                    EstadoAnterior = ObtenerTextoEstado(m.EstadoAnterior),
                    EstadoNuevo = ObtenerTextoEstado(m.EstadoNuevo),
                    Motivo = m.Motivo,
                    OrigenDestino = m.ClienteNombre,
                    ClienteNombre = m.ClienteNombre,
                    Observaciones = m.Observaciones,
                    LoteId = m.LoteId
                }).ToList()
            };
        }

        private static BarrilDto MapToDto(Barril b) => new()
        {
            Id = b.Id,
            Codigo = b.Codigo,
            FormatoEnvaseId = b.FormatoEnvaseId,
            NombreFormato = b.FormatoEnvase?.Nombre ?? string.Empty,
            CapacidadLitros = b.FormatoEnvase?.CapacidadLitros ?? 0,
            Estado = (int)b.Estado,
            EstadoTexto = ObtenerTextoEstado(b.Estado),
            ClienteId = b.ClienteId,
            ClienteNombre = b.Cliente?.RazonSocial,
            FechaAdquisicion = b.FechaAdquisicion,
            UltimaActualizacion = b.UltimaActualizacion,
            Observaciones = b.Observaciones,
        };

        private static string ObtenerTextoEstado(EstadoBarril estado) => estado switch
        {
            EstadoBarril.Disponible => "Disponible",
            EstadoBarril.Lleno => "Lleno",
            EstadoBarril.ConCliente => "Con Cliente",
            EstadoBarril.Sucio => "Sucio",
            EstadoBarril.EnLavado => "En Lavado",
            EstadoBarril.Mantenimiento => "Mantenimiento",
            _ => "Desconocido"
        };

        public async Task<bool> RegistrarMovimientoAsync(RegistrarMovimientoDto dto)
        {
            var barril = await _repository.GetByIdAsync(dto.BarrilId);
            if (barril == null) return false;

            var tipo = (TipoMovimientoBarril)dto.TipoMovimiento;
            var estadoAnterior = barril.Estado;
            var estadoNuevo = barril.Estado;

            switch (tipo)
            {
                case TipoMovimientoBarril.DespachoCliente:
                    estadoNuevo = EstadoBarril.ConCliente;
                    barril.ClienteId = dto.ClienteId;
                    break;
                case TipoMovimientoBarril.DevolucionCliente:
                    estadoNuevo = EstadoBarril.Sucio;
                    barril.ClienteId = null;
                    barril.LoteActualId = null;
                    break;
                case TipoMovimientoBarril.IngresoLavadero:
                    estadoNuevo = EstadoBarril.EnLavado;
                    break;
                case TipoMovimientoBarril.FinLavado:
                    estadoNuevo = EstadoBarril.Disponible;
                    break;
                case TipoMovimientoBarril.EnvioMantenimiento:
                    estadoNuevo = EstadoBarril.Mantenimiento;
                    break;
                case TipoMovimientoBarril.RetornoMantenimiento:
                    estadoNuevo = EstadoBarril.Disponible;
                    break;
            }

            var movimiento = new MovimientoBarril
            {
                BarrilId = dto.BarrilId,
                Fecha = DateTime.Now,
                EstadoAnterior = estadoAnterior,
                EstadoNuevo = estadoNuevo,
                Motivo = tipo.ToString(),
                ClienteNombre = dto.ClienteNombre,
                Observaciones = dto.Observaciones, 
                LoteId = barril.LoteActualId
            };

            barril.Estado = estadoNuevo;
            barril.Movimientos.Add(movimiento);
            barril.UltimaActualizacion = DateTime.Now;

            await _repository.UpdateAsync(barril);

            return true;
        }

        public async Task<bool> EliminarUltimoMovimientoAsync(int barrilId)
        {
            var barril = await _repository.ObtenerDetalleAsync(barrilId);
            if (barril == null || barril.Movimientos == null || !barril.Movimientos.Any()) return false;

            var ultimoMovimiento = barril.Movimientos.OrderByDescending(m => m.Fecha).First();

            barril.Estado = ultimoMovimiento.EstadoAnterior;
            if (barril.Estado != EstadoBarril.ConCliente)
            {
                barril.ClienteId = null;
            }

            barril.Movimientos.Remove(ultimoMovimiento);
            barril.UltimaActualizacion = DateTime.Now;

            return await _repository.UpdateAsync(barril);
        }
    }
}