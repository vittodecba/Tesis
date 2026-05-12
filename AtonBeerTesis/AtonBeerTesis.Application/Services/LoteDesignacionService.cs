using AtonBeerTesis.Application.Dtos.STOCK;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using AtonBeerTesis.Domain.Interfaces;

namespace AtonBeerTesis.Application.Services
{
    public class LoteDesignacionService : ILoteDesignacionService
    {
        private readonly IRepository<LoteDesignacion> _designacionRepository;
        private readonly IRepository<FormatoEnvase> _formatoRepository;
        private readonly ILoteRepository _loteRepository;

        public LoteDesignacionService(
            IRepository<LoteDesignacion> designacionRepository,
            IRepository<FormatoEnvase> formatoRepository,
            ILoteRepository loteRepository)
        {
            _designacionRepository = designacionRepository;
            _formatoRepository = formatoRepository;
            _loteRepository = loteRepository;
        }

        public async Task<IEnumerable<LoteDesignacionDto>> ObtenerPorLoteAsync(int loteId)
        {
            var todas = await _designacionRepository.FindAllAsync();
            var formatos = await _formatoRepository.FindAllAsync();

            return todas
                .Where(d => d.LoteId == loteId)
                .Select(d =>
                {
                    var formato = formatos.FirstOrDefault(f => f.Id == d.FormatoEnvaseId);
                    return new LoteDesignacionDto
                    {
                        Id = d.Id,
                        FormatoEnvaseId = d.FormatoEnvaseId,
                        NombreFormato = formato?.Nombre ?? "Desconocido",
                        CapacidadLitros = formato?.CapacidadLitros ?? 0,
                        VolumenAsignado = d.VolumenAsignado
                    };
                });
        }

        public async Task<LoteDesignacionDto> AgregarDesignacionAsync(int loteId, CreateLoteDesignacionDto dto)
        {
            var lote = await _loteRepository.GetByIdAsync(loteId)
                ?? throw new Exception("Lote no encontrado");

            if (lote.Estado == EstadoLote.Finalizado || lote.Estado == EstadoLote.Descartado)
                throw new Exception("No se pueden agregar designaciones a un lote finalizado o descartado");

            var formato = await _formatoRepository.FindOneAsync(dto.FormatoEnvaseId)
                ?? throw new Exception("Formato de envase no encontrado");

            // Validación 1: el volumen debe ser divisible exactamente por la capacidad del envase
            if (dto.VolumenAsignado <= 0)
                throw new Exception("El volumen asignado debe ser mayor a 0");

            var resto = dto.VolumenAsignado % formato.CapacidadLitros;
            if (resto != 0)
                throw new Exception(
                    $"No se puede repartir {dto.VolumenAsignado}L en envases de {formato.CapacidadLitros}L. " +
                    $"El volumen debe ser múltiplo de {formato.CapacidadLitros}L");

            // Validación 2: la suma de designaciones no puede superar el volumen del lote
            var designacionesExistentes = await _designacionRepository.FindAllAsync();
            var volumenYaDesignado = designacionesExistentes
                .Where(d => d.LoteId == loteId)
                .Sum(d => d.VolumenAsignado);

            if (volumenYaDesignado + dto.VolumenAsignado > lote.VolumenLitros)
                throw new Exception(
                    $"El volumen total designado ({volumenYaDesignado + dto.VolumenAsignado}L) " +
                    $"supera el volumen del lote ({lote.VolumenLitros}L)");

            var designacionExistente = designacionesExistentes
                .FirstOrDefault(d => d.LoteId == loteId && d.FormatoEnvaseId == dto.FormatoEnvaseId);

            if (designacionExistente != null)
            {
                designacionExistente.VolumenAsignado += dto.VolumenAsignado;
                _designacionRepository.Update(designacionExistente.Id, designacionExistente);
                return new LoteDesignacionDto
                {
                    Id = designacionExistente.Id,
                    FormatoEnvaseId = formato.Id,
                    NombreFormato = formato.Nombre,
                    CapacidadLitros = formato.CapacidadLitros,
                    VolumenAsignado = designacionExistente.VolumenAsignado
                };
            }

            var designacion = new LoteDesignacion
            {
                LoteId = loteId,
                FormatoEnvaseId = dto.FormatoEnvaseId,
                VolumenAsignado = dto.VolumenAsignado
            };
            await _designacionRepository.AddAsync(designacion);

            return new LoteDesignacionDto
            {
                Id = designacion.Id,
                FormatoEnvaseId = formato.Id,
                NombreFormato = formato.Nombre,
                CapacidadLitros = formato.CapacidadLitros,
                VolumenAsignado = dto.VolumenAsignado
            };
        }

        public async Task<bool> EliminarDesignacionAsync(int id)
        {
            var designacion = await _designacionRepository.FindOneAsync(id);
            if (designacion == null) return false;

            _designacionRepository.Remove(id);
            return true;
        }
    }
}
