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

            // Convertimos de Entidad a DTO (Sin el campo Tipo)
            return lista.Select(f => new FermentadorDto
            {
                Id = f.Id,
                Nombre = f.Nombre,
                Capacidad = f.Capacidad,
                Estado = f.Estado.ToString(),
                Observaciones = f.Observaciones
            }).ToList();
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