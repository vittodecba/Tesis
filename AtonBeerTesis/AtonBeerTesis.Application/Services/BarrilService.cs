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
    }

    public class BarrilService : IBarrilService
    {
        private readonly IBarrilRepository _repository;

        // Transiciones válidas por estado. Disponible→Lleno es exclusivo del lote (PBI C).
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
                Codigo           = dto.Codigo.Trim().ToUpper(),
                FormatoEnvaseId  = dto.FormatoEnvaseId,
                Estado           = EstadoBarril.Disponible,
                FechaAdquisicion = dto.FechaAdquisicion,
                Observaciones    = dto.Observaciones,
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

            if (dto.FechaAdquisicion.HasValue)
                barril.FechaAdquisicion = dto.FechaAdquisicion.Value;

            barril.Observaciones = dto.Observaciones;

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
            FechaAdquisicion= b.FechaAdquisicion,
            Observaciones   = b.Observaciones,
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
