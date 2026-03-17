using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Services
{
    public class LoteService : ILoteService
    {
        private readonly ILoteRepository _repository;
        private readonly IFermentadorRepository _fermentadorRepository;

        public LoteService(ILoteRepository repository, IFermentadorRepository fermentadorRepository)
        {
            _repository = repository;
            _fermentadorRepository = fermentadorRepository;
        }

        public async Task<List<LoteDto>> GetAllAsync()
        {
            var lotes = await _repository.GetAllAsync();

            return lotes.Select(l => new LoteDto
            {
                Id = l.Id,
                Codigo = l.Codigo,
                RecetaId = l.RecetaId,
                RecetaNombre = l.Receta?.Nombre,
                FermentadorId = l.FermentadorId,
                FermentadorNombre = l.Fermentador?.Nombre,
                FechaElaboracion = l.FechaElaboracion,
                Estilo = l.Estilo,
                Inoculo = l.Inoculo,
                Responsable = l.Responsable,
                DiasEstimadosFermentacion = l.DiasEstimadosFermentacion,
                Estado = l.Estado,
                Observaciones = l.Observaciones,
                FechaFinReal = l.FechaFinReal
            }).ToList();
        }

        public async Task<LoteDetalleDto?> GetByIdAsync(int id)
        {
            var lote = await _repository.GetByIdAsync(id);
            if (lote == null) return null;

            var ultimoRegistro = lote.RegistrosFermentacion
                .OrderByDescending(r => r.DiaFermentacion)
                .FirstOrDefault();

            return new LoteDetalleDto
            {
                Id = lote.Id,
                Codigo = lote.Codigo,
                RecetaId = lote.RecetaId,
                RecetaNombre = lote.Receta?.Nombre,
                FermentadorId = lote.FermentadorId,
                FermentadorNombre = lote.Fermentador?.Nombre,
                FechaElaboracion = lote.FechaElaboracion,
                Estilo = lote.Estilo,
                Inoculo = lote.Inoculo,
                Responsable = lote.Responsable,
                DiasEstimadosFermentacion = lote.DiasEstimadosFermentacion,
                Estado = lote.Estado,
                Observaciones = lote.Observaciones,
                FechaFinReal = lote.FechaFinReal,
                CantidadRegistros = lote.RegistrosFermentacion.Count,
                UltimoPh = ultimoRegistro?.Ph,
                UltimaDensidad = ultimoRegistro?.Densidad,
                UltimaTemperatura = ultimoRegistro?.Temperatura,
                UltimaPresion = ultimoRegistro?.Presion
            };
        }

        public async Task<LoteDto?> GetActivoByFermentadorIdAsync(int fermentadorId)
        {
            var lote = await _repository.GetActivoByFermentadorIdAsync(fermentadorId);
            if (lote == null) return null;

            return new LoteDto
            {
                Id = lote.Id,
                Codigo = lote.Codigo,
                RecetaId = lote.RecetaId,
                RecetaNombre = lote.Receta?.Nombre,
                FermentadorId = lote.FermentadorId,
                FermentadorNombre = lote.Fermentador?.Nombre,
                FechaElaboracion = lote.FechaElaboracion,
                Estilo = lote.Estilo,
                Inoculo = lote.Inoculo,
                Responsable = lote.Responsable,
                DiasEstimadosFermentacion = lote.DiasEstimadosFermentacion,
                Estado = lote.Estado,
                Observaciones = lote.Observaciones,
                FechaFinReal = lote.FechaFinReal
            };
        }

        public async Task<LoteDto> CreateAsync(CreateLoteDto dto)
        {
            var existeCodigo = await _repository.ExisteCodigoAsync(dto.Codigo);
            if (existeCodigo)
                throw new Exception("Ya existe un lote con ese código.");

            var fermentador = await _fermentadorRepository.GetByIdAsync(dto.FermentadorId);
            if (fermentador == null)
                throw new Exception("Fermentador no encontrado.");

            if (fermentador.Estado != EstadoFermentador.Disponible)
                throw new Exception("El fermentador no está disponible.");

            var loteActivo = await _repository.GetActivoByFermentadorIdAsync(dto.FermentadorId);
            if (loteActivo != null)
                throw new Exception("Ese fermentador ya tiene un lote en proceso.");

            var lote = new LotePrueba
            {
                Codigo = dto.Codigo,
                RecetaId = dto.RecetaId,
                FermentadorId = dto.FermentadorId,
                FechaElaboracion = dto.FechaElaboracion,
                Estilo = dto.Estilo,
                Inoculo = dto.Inoculo,
                Responsable = dto.Responsable,
                DiasEstimadosFermentacion = dto.DiasEstimadosFermentacion,
                Observaciones = dto.Observaciones,
                PlanificacionProduccionId = dto.PlanificacionProduccionId,
                Estado = "EnProceso"
            };

            await _repository.AddAsync(lote);

            fermentador.Estado = EstadoFermentador.Ocupado;
            await _fermentadorRepository.UpdateAsync(fermentador);

            return new LoteDto
            {
                Id = lote.Id,
                Codigo = lote.Codigo,
                RecetaId = lote.RecetaId,
                FermentadorId = lote.FermentadorId,
                FechaElaboracion = lote.FechaElaboracion,
                Estilo = lote.Estilo,
                Inoculo = lote.Inoculo,
                Responsable = lote.Responsable,
                DiasEstimadosFermentacion = lote.DiasEstimadosFermentacion,
                Estado = lote.Estado,
                Observaciones = lote.Observaciones
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateLoteDto dto)
        {
            var lote = await _repository.GetByIdAsync(id);
            if (lote == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Codigo))
                lote.Codigo = dto.Codigo;

            if (dto.RecetaId.HasValue)
                lote.RecetaId = dto.RecetaId.Value;

            if (dto.FechaElaboracion.HasValue)
                lote.FechaElaboracion = dto.FechaElaboracion.Value;

            if (dto.Estilo != null)
                lote.Estilo = dto.Estilo;

            if (dto.Inoculo != null)
                lote.Inoculo = dto.Inoculo;

            if (dto.Responsable != null)
                lote.Responsable = dto.Responsable;

            if (dto.DiasEstimadosFermentacion.HasValue)
                lote.DiasEstimadosFermentacion = dto.DiasEstimadosFermentacion.Value;

            if (dto.Observaciones != null)
                lote.Observaciones = dto.Observaciones;

            if (!string.IsNullOrWhiteSpace(dto.Estado))
                lote.Estado = dto.Estado;

            return await _repository.UpdateAsync(lote);
        }

        public async Task<bool> FinalizarAsync(int id)
        {
            var lote = await _repository.GetByIdAsync(id);
            if (lote == null) return false;

            lote.Estado = "Finalizado";
            lote.FechaFinReal = DateTime.Now;

            var updated = await _repository.UpdateAsync(lote);
            if (!updated) return false;

            var fermentador = await _fermentadorRepository.GetByIdAsync(lote.FermentadorId);
            if (fermentador != null)
            {
                fermentador.Estado = EstadoFermentador.Disponible;
                await _fermentadorRepository.UpdateAsync(fermentador);
            }

            return true;
        }
    }
}