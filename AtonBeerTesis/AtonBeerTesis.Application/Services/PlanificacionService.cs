using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AtonBeerTesis.Application.Services
{
    public class PlanificacionService : IPlanificacionService
    {
        private readonly IPlanificacionRepository _repository;
        private readonly ILoteRepository _loteRepository;
        public PlanificacionService(IPlanificacionRepository repository, ILoteRepository loteRepository)
        {
            _repository = repository;
            _loteRepository = loteRepository;
        }
        public async Task<PlanificacionProduccionDto> PLanificarProduccion(PlanificacionProduccionDto dto)
        {
            //Validaciones
            //Fecha actual
            if(dto.FechaInicio < DateTime.Now.Date)
            {
                throw new Exception("La fecha de producción no puede ser menor a la fecha actual.");
            }
            if (dto.FechaFinEstimada <= dto.FechaInicio)
            {
                throw new Exception("La fecha fin de producción no puede ser menor o igual a la de inicio");
            }
            //Fermentadores
            var ocupado = await _repository.ExisteFermentadorOcupado(dto.FermentadorId, dto.FechaInicio, dto.FechaFinEstimada);
            if (ocupado)
            {
                throw new Exception("El fermentador seleccionado ya está ocupado en la fecha indicada.");
            }
            var nuevoLote = new Lote
            {
                RecetaId = dto.RecetaId,
                VolumenLitros = dto.VolumenLitros,
                CodigoLote = $"L-{DateTime.Now:yyyyMMdd}-{new Random().Next(100, 999)}",
                Estado = EstadoLote.Planificado,
              FechaCreacion = DateTime.Now
            };
            var loteGuardado = await _loteRepository.CreateAsync(nuevoLote);
            var mensajeError = await StockSuficientePorLote(loteGuardado.Id);
            if (mensajeError != null) 
            {
                throw new Exception(mensajeError);
            }
            //Mapeo del dto
            var planificacion = new PlanificacionProduccion
            {
                LoteId = loteGuardado.Id,
                FermentadorId = dto.FermentadorId,
                FechaInicio = dto.FechaInicio,
                FechaFinEstimada = dto.FechaFinEstimada,
                Observaciones = dto.Observaciones,
                UsuarioId = dto.UsuarioId,
                Estado = EstadoLote.Planificado,
                FechaCreacion = DateTime.Now
              
            };
            await _repository.CreateAsync(planificacion);
            dto.LoteId = loteGuardado.Id;//Actualiza el DTO con el ID del lote creado
            return dto;
        }
        public async Task<IEnumerable<PlanificacionProduccionDto>> GetAllAsync()
        {
            var planificaicon = await _repository.GetAllAsync();
            return planificaicon.Select(p => new PlanificacionProduccionDto
            {
                LoteId = p.LoteId,
                FermentadorId = p.FermentadorId,
                FechaInicio = p.FechaInicio,
                FechaFinEstimada = p.FechaFinEstimada,
                Observaciones = p.Observaciones,
                UsuarioId = p.UsuarioId,
                RecetaId = p.Lote !=null ? p.Lote.RecetaId : 0,// Si el lote es null, asigna 0 o un valor predeterminado
                VolumenLitros = p.Lote != null ? p.Lote.VolumenLitros : 0, // Si el lote es null, asigna 0 o un valor predeterminado
                Estado = p.Estado
            }).ToList();
        }
        //Validacion del insumo que sea suficiente para poder asignarse en el plan de produccion
        public async Task<string> StockSuficientePorLote(int LoteId)
        {
            var insumosRequeridos = await _loteRepository.GetRecetaInsumosByLoteIdAsync(LoteId);
            var lote = await _loteRepository.GetByIdAsync(LoteId);
            if (!insumosRequeridos.Any())
                return "La receta seleccionada no contiene insumos cargados.";
            foreach (var i in insumosRequeridos)
            {
                //Calculo de la cantidad necesaria del insumo para el lote, basado en la cantidad requerida por la receta y el volumen del lote a producir.
                //Esto es necesario para comparar con el stock actual del insumo y determinar si hay suficiente para planificar la producción.
                decimal cantidadNecesaria = (i.Cantidad / lote.Receta.BatchSizeLitros) * lote.VolumenLitros;
                if (cantidadNecesaria > i.Insumo.StockActual)
                {
                    return $"Falta de stock de {i.Insumo.NombreInsumo}. Necesitas {cantidadNecesaria:F2}. Actualmente hay {i.Insumo.StockActual:F2}";//F:2 para mostrar 2 decimales
                }
            }
            return null;
        }

        public async Task<PlanificacionProduccionDto> ActualizarPlanificacion(int loteId, PlanificacionProduccionDto dto)
        {
            var planificacion = await _repository.GetByIdAsync(loteId);
            if (planificacion == null)
                throw new Exception($"No se encontró la planificación con Lote ID {loteId}.");
            
            if (dto.FechaInicio != planificacion.FechaInicio && dto.FechaInicio < DateTime.Now.Date)
                throw new Exception("La fecha de inicio no puede ser menor a la fecha actual."); 

            if (dto.FechaFinEstimada <= dto.FechaInicio)
                throw new Exception("La fecha fin no puede ser menor o igual a la de inicio.");           

            // Verificar fermentador ocupado solo si cambió el fermentador o las fechas
            bool cambioFermentadorOFechas = planificacion.FermentadorId != dto.FermentadorId ||
                                             planificacion.FechaInicio != dto.FechaInicio ||
                                             planificacion.FechaFinEstimada != dto.FechaFinEstimada;

            if (cambioFermentadorOFechas)
            {
                var ocupado = await _repository.ExisteFermentadorOcupado(dto.FermentadorId, dto.FechaInicio, dto.FechaFinEstimada, loteId);
                if (ocupado)
                    throw new Exception("El fermentador ya está ocupado en ese rango de fechas.");
            }

            // Actualizar campos
            planificacion.FermentadorId = dto.FermentadorId;
            planificacion.FechaInicio = dto.FechaInicio;
            planificacion.FechaFinEstimada = dto.FechaFinEstimada;
            planificacion.Observaciones = dto.Observaciones;
            planificacion.Estado = dto.Estado;

            // Actualizar el lote asociado (volumen y receta)
            if (planificacion.Lote != null)
            {
                planificacion.Lote.RecetaId = dto.RecetaId;
                planificacion.Lote.VolumenLitros = dto.VolumenLitros;
            }

            await _repository.UpdateAsync(planificacion);

            dto.LoteId = loteId;
            return dto;
        }
    }
}
