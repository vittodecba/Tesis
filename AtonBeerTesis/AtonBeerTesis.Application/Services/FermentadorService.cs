using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;

namespace AtonBeerTesis.Application.Services
{
    public interface IFermentadorService
    {
        Task<List<FermentadorDto>> GetAllAsync();
        Task<FermentadorDto> CreateAsync(CreateFermentadorDto dto);
        Task<bool> UpdateAsync(int id, UpdateFermentadorDto dto);
    }

    public class FermentadorService : IFermentadorService
    {
        private readonly IFermentadorRepository _repository;

        public FermentadorService(IFermentadorRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FermentadorDto>> GetAllAsync()
        {
            var lista = await _repository.GetAllAsync();
            return lista.Select(f => new FermentadorDto
            {
                Id = f.Id,
                Nombre = f.Nombre,
                Capacidad = f.Capacidad,
                Estado = f.Estado.ToString(),
                Observaciones = f.Observaciones
            }).ToList();
        }

        public async Task<bool> UpdateAsync(int id, UpdateFermentadorDto dto)
        {
            // 1. Buscamos el fermentador que ya está guardado
            var fermentadorExistente = await _repository.GetByIdAsync(id);
            if (fermentadorExistente == null) return false;

            // 2. Mapeo Inteligente (PATCH): Solo actualiza si hay un valor nuevo
            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                fermentadorExistente.Nombre = dto.Nombre;

            // Usamos .HasValue porque ahora Capacidad es int?
            if (dto.Capacidad.HasValue)
                fermentadorExistente.Capacidad = dto.Capacidad.Value;

            // Dentro de tu método UpdateAsync:
            if (dto.Estado.HasValue)
            {
                // Casteamos el número recibido al Enum de la entidad
                fermentadorExistente.Estado = (EstadoFermentador)dto.Estado.Value;
            }

            if (dto.Observaciones != null)
                fermentadorExistente.Observaciones = dto.Observaciones;

            // 3. Guardamos solo los cambios
            return await _repository.UpdateAsync(fermentadorExistente);
        }

        public async Task<FermentadorDto> CreateAsync(CreateFermentadorDto dto)
        {
            var nuevo = new Fermentador
            {
                Nombre = dto.Nombre,
                Capacidad = dto.Capacidad,
                Observaciones = dto.Observaciones,
                Estado = EstadoFermentador.Disponible
            };

            await _repository.AddAsync(nuevo);

            return new FermentadorDto
            {
                Id = nuevo.Id,
                Nombre = nuevo.Nombre,
                Capacidad = nuevo.Capacidad,
                Estado = nuevo.Estado.ToString(),
                Observaciones = nuevo.Observaciones
            };
        }
    }
}