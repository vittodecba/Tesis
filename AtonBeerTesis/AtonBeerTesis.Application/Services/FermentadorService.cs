using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Application.Services
{
    public interface IFermentadorService
    {
        Task<List<FermentadorDto>> GetAllAsync();
        Task<FermentadorDto> CreateAsync(CreateFermentadorDto dto);
        Task<bool> UpdateAsync(int id, UpdateFermentadorDto dto);
    }


namespace AtonBeerTesis.Application.Services
    {
        public class FermentadorService : IFermentadorService
        {
            private readonly IFermentadorRepository _repository;

            public FermentadorService(IFermentadorRepository repository)
            {
                _repository = repository;
            }

            public async Task<IEnumerable<FermentadorDetalleDto>> GetAllConLoteAsync()
            {
                var fermentadores = await _repository.GetAllConPlanificacionAsync();

                return fermentadores.Select(f => new FermentadorDetalleDto
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Capacidad = f.Capacidad,
                    Estado = ((int)f.Estado).ToString(),
                    Observaciones = f.Observaciones,
                    LoteId = null,
                    EstiloNombre = null
                }).ToList();
            }

            public async Task<List<FermentadorDto>> GetAllAsync()
            {
                var lista = await _repository.GetAllAsync();

                return lista.Select(f => new FermentadorDto
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Capacidad = f.Capacidad,
                    Estado = ((int)f.Estado).ToString(),
                    Observaciones = f.Observaciones,
                    LoteId = null,
                    EstiloNombre = null
                }).ToList();
            }

            public async Task<bool> UpdateAsync(int id, UpdateFermentadorDto dto)
            {
                var fermentadorExistente = await _repository.GetByIdAsync(id);
                if (fermentadorExistente == null) return false;

                if (!string.IsNullOrWhiteSpace(dto.Nombre))
                    fermentadorExistente.Nombre = dto.Nombre;

                if (dto.Capacidad.HasValue)
                    fermentadorExistente.Capacidad = dto.Capacidad.Value;

                if (dto.Estado.HasValue)
                {
                    var nuevoEstado = (EstadoFermentador)dto.Estado.Value;

                    if (fermentadorExistente.Estado == EstadoFermentador.Ocupado &&
                        nuevoEstado != EstadoFermentador.Ocupado)
                    {
                        throw new Exception("Un fermentador ocupado no puede cambiarse manualmente de estado. Debe finalizarse el lote primero.");
                    }

                    if (nuevoEstado == EstadoFermentador.Mantenimiento &&
                        fermentadorExistente.Estado != EstadoFermentador.Disponible)
                    {
                        throw new Exception("Solo se puede pasar a mantenimiento desde el estado disponible.");
                    }

                    fermentadorExistente.Estado = nuevoEstado;
                }

                if (dto.Observaciones != null)
                    fermentadorExistente.Observaciones = dto.Observaciones;

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
                    Estado = ((int)nuevo.Estado).ToString(),
                    Observaciones = nuevo.Observaciones,
                    LoteId = null,
                    EstiloNombre = null
                };
            }
        }
    }
}