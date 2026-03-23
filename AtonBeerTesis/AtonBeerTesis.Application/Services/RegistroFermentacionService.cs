using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;

namespace AtonBeerTesis.Application.Services
{
    public class RegistroFermentacionService : IRegistroFermentacionService
    {
        private readonly IRegistroFermentacionRepository _repository;
        private readonly ILoteRepository _loteRepository;

        public RegistroFermentacionService(
            IRegistroFermentacionRepository repository,
            ILoteRepository loteRepository)
        {
            _repository = repository;
            _loteRepository = loteRepository;
        }

        public async Task<List<RegistroFermentacionDto>> GetByLoteIdAsync(int loteId)
        {
            var registros = await _repository.GetByLoteIdAsync(loteId);

            return registros
                .OrderBy(r => r.DiaFermentacion)
                .Select(r => new RegistroFermentacionDto
                {
                    Id = r.Id,
                    LoteId = r.LoteId,
                    Fecha = r.Fecha,
                    DiaFermentacion = r.DiaFermentacion,
                    Ph = r.Ph,
                    Densidad = r.Densidad,
                    Temperatura = r.Temperatura,
                    Presion = r.Presion,
                    Purgas = r.Purgas,
                    Extracciones = r.Extracciones,
                    Agregados = r.Agregados,
                    Observaciones = r.Observaciones
                }).ToList();
        }

        public async Task<RegistroFermentacionDto> CreateAsync(CreateRegistroFermentacionDto dto)
        {
            var lote = await _loteRepository.GetByIdAsync(dto.LoteId);
            if (lote == null)
                throw new Exception("Lote no encontrado.");

            if (lote.Estado != "EnProceso")
                throw new Exception("Solo se pueden cargar registros a lotes en proceso.");

            if (dto.DiaFermentacion <= 0)
                throw new Exception("El día de fermentación debe ser mayor a 0.");

            if (dto.Ph <= 0)
                throw new Exception("El pH debe ser mayor a 0.");

            if (dto.Temperatura < 0)
                throw new Exception("La temperatura no puede ser negativa.");

            if (dto.Densidad < 0)
                throw new Exception("La densidad no puede ser negativa.");

            if (!dto.Presion.HasValue)
                throw new Exception("La presión es obligatoria.");

            if (dto.Presion.Value < 0)
                throw new Exception("La presión no puede ser negativa.");

            var existeFecha = await _repository.ExistePorFechaAsync(dto.LoteId, dto.Fecha.Date);
            if (existeFecha)
                throw new Exception("Ya existe un registro para esa fecha.");

            var existeDia = await _repository.ExistePorDiaAsync(dto.LoteId, dto.DiaFermentacion);
            if (existeDia)
                throw new Exception("Ya existe un registro para ese día de fermentación.");

            var registro = new RegistroFermentacion
            {
                LoteId = dto.LoteId,
                Fecha = dto.Fecha.Date,
                DiaFermentacion = dto.DiaFermentacion,
                Ph = dto.Ph,
                Densidad = dto.Densidad,
                Temperatura = dto.Temperatura,
                Presion = dto.Presion,
                Purgas = dto.Purgas,
                Extracciones = dto.Extracciones,
                Agregados = dto.Agregados,
                Observaciones = dto.Observaciones
            };

            await _repository.AddAsync(registro);

            return new RegistroFermentacionDto
            {
                Id = registro.Id,
                LoteId = registro.LoteId,
                Fecha = registro.Fecha,
                DiaFermentacion = registro.DiaFermentacion,
                Ph = registro.Ph,
                Densidad = registro.Densidad,
                Temperatura = registro.Temperatura,
                Presion = registro.Presion,
                Purgas = registro.Purgas,
                Extracciones = registro.Extracciones,
                Agregados = registro.Agregados,
                Observaciones = registro.Observaciones
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateRegistroFermentacionDto dto)
        {
            var registro = await _repository.GetByIdAsync(id);
            if (registro == null) return false;

            if (dto.Fecha.HasValue)
                registro.Fecha = dto.Fecha.Value.Date;

            if (dto.DiaFermentacion.HasValue)
            {
                if (dto.DiaFermentacion.Value <= 0)
                    throw new Exception("El día de fermentación debe ser mayor a 0.");

                registro.DiaFermentacion = dto.DiaFermentacion.Value;
            }

            if (dto.Ph.HasValue)
            {
                if (dto.Ph.Value <= 0)
                    throw new Exception("El pH debe ser mayor a 0.");

                registro.Ph = dto.Ph.Value;
            }

            if (dto.Densidad.HasValue)
            {
                if (dto.Densidad.Value < 0)
                    throw new Exception("La densidad no puede ser negativa.");

                registro.Densidad = dto.Densidad.Value;
            }

            if (dto.Temperatura.HasValue)
            {
                if (dto.Temperatura.Value < 0)
                    throw new Exception("La temperatura no puede ser negativa.");

                registro.Temperatura = dto.Temperatura.Value;
            }

            if (dto.Presion.HasValue)
            {
                if (dto.Presion.Value < 0)
                    throw new Exception("La presión no puede ser negativa.");

                registro.Presion = dto.Presion.Value;
            }

            if (dto.Purgas != null)
                registro.Purgas = dto.Purgas;

            if (dto.Extracciones != null)
                registro.Extracciones = dto.Extracciones;

            if (dto.Agregados != null)
                registro.Agregados = dto.Agregados;

            if (dto.Observaciones != null)
                registro.Observaciones = dto.Observaciones;

            return await _repository.UpdateAsync(registro);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var registro = await _repository.GetByIdAsync(id);
            if (registro == null) return false;

            return await _repository.DeleteAsync(registro);
        }
    }
}