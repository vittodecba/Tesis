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
        Task<IEnumerable<FermentadorDetalleDto>> GetAllConLoteAsync();
        Task<FermentadorDto> CreateAsync(CreateFermentadorDto dto);
        Task<bool> UpdateAsync(int id, UpdateFermentadorDto dto);
        Task<bool> DeleteAsync(int id);
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

            public async Task<bool> DeleteAsync(int id)
            {
                var fermentador = await _repository.GetByIdAsync(id);
                if (fermentador == null) return false;

                if (fermentador.Estado == EstadoFermentador.Ocupado)
                    throw new Exception("No se puede eliminar un fermentador ocupado. Debe finalizarse el lote primero.");

                if (await _repository.TieneLotesAsociadosAsync(id))
                    throw new Exception("No se puede eliminar este fermentador porque tiene lotes asociados.");

                return await _repository.DeleteAsync(id);
            }

            public async Task<IEnumerable<FermentadorDetalleDto>> GetAllConLoteAsync()
            {
                var fermentadores = await _repository.GetAllConPlanificacionAsync();

                return fermentadores.Select(f =>
                {
                    // Buscar la planificación activa (Planificado=1 o EnProceso=2)
                    var planActiva = SeleccionarPlanActiva(f.Planificaciones);

                    return new FermentadorDetalleDto
                    {
                        Id = f.Id,
                        Nombre = f.Nombre,
                        Capacidad = f.Capacidad,
                        Estado = ((int)f.Estado).ToString(),
                        Observaciones = f.Observaciones,
                        LoteId = planActiva?.LoteId,
                        EstiloNombre = planActiva?.Lote?.Receta?.Estilo,
                        CodigoLote = planActiva?.Lote?.CodigoLote,
                        VolumenLitrosLote = planActiva?.Lote?.VolumenLitros,
                        EstadoLote = planActiva != null ? ((int)planActiva.Estado).ToString() : null
                    };
                }).ToList();
            }

            public async Task<List<FermentadorDto>> GetAllAsync()
            {
                // Usamos GetAllConPlanificacionAsync para poder poblar los campos de lote activo
                var lista = await _repository.GetAllConPlanificacionAsync();

                return lista.Select(f =>
                {
                    var planActiva = SeleccionarPlanActiva(f.Planificaciones);

                    return new FermentadorDto
                    {
                        Id = f.Id,
                        Nombre = f.Nombre,
                        Capacidad = f.Capacidad,
                        Estado = ((int)f.Estado).ToString(),
                        Observaciones = f.Observaciones,
                        LoteId = planActiva?.LoteId,
                        EstiloNombre = planActiva?.Lote?.Receta?.Estilo,
                        CodigoLote = planActiva?.Lote?.CodigoLote,
                        VolumenLitrosLote = planActiva?.Lote?.VolumenLitros,
                        EstadoLote = planActiva != null ? ((int)planActiva.Estado).ToString() : null
                    };
                }).ToList();
            }

            // Selecciona la planificación "activa" del fermentador priorizando la que está
            // EnProceso (la realmente en curso) y, dentro de cada estado, la más reciente. Evita
            // que un Planificado viejo le gane al lote en proceso cuando hay varias planificaciones
            // activas asignadas al mismo fermentador.
            private static PlanificacionProduccion? SeleccionarPlanActiva(IEnumerable<PlanificacionProduccion> planificaciones)
            {
                return planificaciones
                    .Where(p => p.Estado == EstadoLote.EnProceso || p.Estado == EstadoLote.Planificado)
                    .OrderBy(p => p.Estado == EstadoLote.EnProceso ? 0 : 1) // EnProceso primero
                    .ThenByDescending(p => p.LoteId)                         // el más reciente
                    .FirstOrDefault();
            }

            public async Task<bool> UpdateAsync(int id, UpdateFermentadorDto dto)
            {
                var fermentadorExistente = await _repository.GetByIdAsync(id);
                if (fermentadorExistente == null) return false;

                // Bloquear edición solo si el fermentador está Ocupado (hay un lote corriendo activamente)
                if (fermentadorExistente.Estado == EstadoFermentador.Ocupado)
                    throw new Exception("No se puede editar un fermentador con un lote activo asignado. Finalizá el lote primero.");

                if (!string.IsNullOrWhiteSpace(dto.Nombre))
                {
                    if (await _repository.ExisteNombreAsync(dto.Nombre, id))
                        throw new Exception($"Ya existe un fermentador con el nombre '{dto.Nombre}'.");
                    fermentadorExistente.Nombre = dto.Nombre;
                }

                if (dto.Capacidad.HasValue)
                    fermentadorExistente.Capacidad = dto.Capacidad.Value;

                if (dto.Estado.HasValue)
                {
                    var nuevoEstado = (EstadoFermentador)dto.Estado.Value;

                    // El estado Ocupado solo se asigna automáticamente al iniciar un lote
                    if (nuevoEstado == EstadoFermentador.Ocupado)
                        throw new Exception("El estado 'Ocupado' se asigna automáticamente al crear un lote.");

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
                // BUG 1: verificar nombre duplicado
                if (await _repository.ExisteNombreAsync(dto.Nombre))
                    throw new Exception($"Ya existe un fermentador con el nombre '{dto.Nombre}'.");

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