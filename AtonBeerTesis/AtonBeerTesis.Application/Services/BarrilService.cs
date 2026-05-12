using AtonBeerTesis.Application.Dtos.BARRIL;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Services
{
    public interface IBarrilService
    {
        Task<List<BarrilDto>> GetAllAsync();
        Task<BarrilDto> CreateAsync(CreateBarrilDto dto);
        Task<bool> UpdateAsync(int id, UpdateBarrilDto dto);
        Task<bool> EliminarAsync(int id);
    }

    public class BarrilService : IBarrilService
    {
        private readonly IBarrilRepository _repository;

        // Transiciones válidas por estado. Disponible→Lleno es exclusivo del lote (PBI C).
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

        private static readonly Dictionary<EstadoBarril, EstadoBarril[]> TransicionesValidas = new()
        {
            [EstadoBarril.Disponible]    = [EstadoBarril.Lleno, EstadoBarril.Mantenimiento],
            [EstadoBarril.Lleno]         = [EstadoBarril.ConCliente],
            [EstadoBarril.ConCliente]    = [EstadoBarril.Sucio],
            [EstadoBarril.Sucio]         = [EstadoBarril.EnLavado],
            [EstadoBarril.EnLavado]      = [EstadoBarril.Disponible],
            [EstadoBarril.Mantenimiento] = [EstadoBarril.Disponible],
        };

        public BarrilService(IBarrilRepository repository)
        {
            _repository = repository;
        }

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
                Codigo               = dto.Codigo.Trim().ToUpper(),
                FormatoEnvaseId      = dto.FormatoEnvaseId,
                Estado               = EstadoBarril.Disponible,
                FechaAdquisicion     = dto.FechaAdquisicion,
                UltimaActualizacion  = DateTime.Now,
                Observaciones        = dto.Observaciones,
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

            barril.Observaciones       = dto.Observaciones;
            barril.UltimaActualizacion = DateTime.Now;

            return await _repository.UpdateAsync(barril);
        }

        private static BarrilDto MapToDto(Barril b) => new()
        {
            Id              = b.Id,
            Codigo          = b.Codigo,
            FormatoEnvaseId = b.FormatoEnvaseId,
            NombreFormato   = b.FormatoEnvase?.Nombre ?? string.Empty,
            CapacidadLitros = b.FormatoEnvase?.CapacidadLitros ?? 0,
            Estado          = (int)b.Estado,
            EstadoTexto     = ObtenerTextoEstado(b.Estado),
            ClienteId       = b.ClienteId,
            ClienteNombre   = b.Cliente?.RazonSocial,
            FechaAdquisicion    = b.FechaAdquisicion,
            UltimaActualizacion = b.UltimaActualizacion,
            Observaciones       = b.Observaciones,
        };

        private static string ObtenerTextoEstado(EstadoBarril estado) => estado switch
        {
            EstadoBarril.Disponible    => "Disponible",
            EstadoBarril.Lleno         => "Lleno",
            EstadoBarril.ConCliente    => "Con Cliente",
            EstadoBarril.Sucio         => "Sucio",
            EstadoBarril.EnLavado      => "En Lavado",
            EstadoBarril.Mantenimiento => "Mantenimiento",
            _                          => "Desconocido"
        };
    }
}
