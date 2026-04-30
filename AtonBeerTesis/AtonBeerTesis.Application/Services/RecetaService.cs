using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Application.Dtos.Recetas;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AtonBeerTesis.Application.Services
{
    public class RecetaService : IRecetaService
    {
        private readonly IRecetaRepository _recetaRepository;

        public RecetaService(IRecetaRepository recetaRepository)
        {
            _recetaRepository = recetaRepository;
        }

        public async Task<List<RecetaDto>> GetAllAsync(string? nombre = null, string? estilo = null, string? estado = null, string? orden = null)
        {
            if (string.IsNullOrWhiteSpace(estado))
            {
                estado = "Activa";
            }

            var recetas = await _recetaRepository.GetAllAsync(nombre, estilo, estado, orden);
            return recetas.Select(MapToDto).ToList();
        }

        public async Task<RecetaDto?> GetByIdAsync(int id)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            return receta is null ? null : MapToDto(receta);
        }

        public async Task<int> CreateAsync(CreateRecetaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new Exception("El nombre de la receta es obligatorio");

            var nombre = dto.Nombre.Trim();

            if (await ExisteNombreAsync(nombre))
                throw new Exception("Ya existe una receta con ese nombre");

            if (dto.BatchSizeLitros <= 0)
                throw new Exception("El volumen (BatchSizeLitros) debe ser mayor a 0");

            var receta = new Receta
            {
                Nombre = nombre,
                Estilo = dto.Estilo?.Trim() ?? "",
                BatchSizeLitros = dto.BatchSizeLitros,
                Notas = dto.Notas?.Trim(),
                Estado = EstadoReceta.Activa,
                FechaCreacion = DateTime.UtcNow,
                RecetaInsumos = dto.RecetaInsumos.Select(i => new RecetaInsumo
                {
                    InsumoId = i.InsumoId,
                    Cantidad = i.Cantidad,
                    unidadMedidaId = i.UnidadMedidaId
                }).ToList()
            };

            await _recetaRepository.AddAsync(receta);
            return receta.IdReceta;
        }

        public async Task<bool> UpdateAsync(int id, ActualizarRecetaDto dto)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            if (receta is null) return false;

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new Exception("El nombre de la receta es obligatorio");

            var nombre = dto.Nombre.Trim();

            if (await ExisteNombreAsync(nombre, id))
                throw new Exception("Ya existe otra receta con ese nombre");

            if (dto.BatchSizeLitros <= 0)
                throw new Exception("El volumen (BatchSizeLitros) debe ser mayor a 0");

            if (!Enum.TryParse<EstadoReceta>(dto.Estado, true, out var estadoEnum))
                throw new Exception("Estado de receta inválido");
            
            // Datos que se actualizan
            receta.Nombre = nombre;
            receta.Estilo = dto.Estilo?.Trim() ?? "";
            receta.BatchSizeLitros = dto.BatchSizeLitros;
            receta.Notas = dto.Notas?.Trim();
            receta.Estado = estadoEnum;
            receta.FechaActualizacion = DateTime.UtcNow;
            if (dto.RecetaInsumos != null && dto.RecetaInsumos.Count > 0)
            {
                // Borramos los que tiene actualmente la receta en memoria
                receta.RecetaInsumos.Clear();

                // Agrego los que vienen del DTO
                foreach (var i in dto.RecetaInsumos)
                {
                    receta.RecetaInsumos.Add(new RecetaInsumo
                    {
                        InsumoId = i.InsumoId,
                        Cantidad = i.Cantidad,
                        unidadMedidaId = i.UnidadMedidaId,
                    });
                }
            }

            await _recetaRepository.UpdateAsync(receta);
            return true;
        }

        public async Task<bool> PatchAsync(int id, PatchRecetaDto dto)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            if (receta is null) return false;

            if (dto.Nombre is not null)
            {
                var nombre = dto.Nombre.Trim();
                if (string.IsNullOrWhiteSpace(nombre)) throw new Exception("El nombre no puede quedar vacío");
                if (await ExisteNombreAsync(nombre, id)) throw new Exception("Ya existe otra receta con ese nombre");
                receta.Nombre = nombre;
            }

            if (dto.Estilo is not null) receta.Estilo = dto.Estilo.Trim();
            if (dto.Notas is not null) receta.Notas = dto.Notas.Trim();
            if (dto.BatchSizeLitros.HasValue)
            {
                if (dto.BatchSizeLitros.Value <= 0) throw new Exception("El volumen debe ser mayor a 0");
                receta.BatchSizeLitros = dto.BatchSizeLitros.Value;
            }

            if (dto.Estado is not null)
            {
                if (!Enum.TryParse<EstadoReceta>(dto.Estado, true, out var estadoEnum)) throw new Exception("Estado inválido");
                receta.Estado = estadoEnum;
            }

            receta.FechaActualizacion = DateTime.UtcNow;
            await _recetaRepository.UpdateAsync(receta);
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            if (receta is null) return false;

            receta.Estado = EstadoReceta.Inactiva;
            receta.FechaActualizacion = DateTime.UtcNow;
            await _recetaRepository.UpdateAsync(receta);
            return true;
        }

        public List<string> GetEstadosReceta() => Enum.GetNames(typeof(EstadoReceta)).ToList();

        private RecetaDto MapToDto(Receta receta)
        {
            return new RecetaDto
            {
                IdReceta = receta.IdReceta,
                Nombre = receta.Nombre,
                Estilo = receta.Estilo,
                BatchSizeLitros = receta.BatchSizeLitros,
                Notas = receta.Notas,
                Estado = receta.Estado.ToString(),
                FechaCreacion = receta.FechaCreacion,
                FechaActualizacion = receta.FechaActualizacion,

                RecetaInsumos = receta.RecetaInsumos?.Select(ri => new RecetaInsumoDto
                {
                    InsumoId = ri.InsumoId,
                    Cantidad = ri.Cantidad,
                    NombreInsumo = ri.Insumo?.NombreInsumo,
                    UnidadMedidaId = ri.unidadMedidaId,
                    UnidadMedida = ri.unidadMedida?.Abreviatura ?? "",
                    StockActual = ri.Insumo?.StockActual ?? 0,//Para poder mostrar el stock actual del insumo en la receta
                    //Esto permite a los usuarios ver cuánto stock tienen disponible de cada insumo  para la receta
                    Factor = (decimal)(ri.unidadMedida?.Factor ?? 1.0),
                    unidadMedidaStock = ri.Insumo?.unidadMedida?.Abreviatura ?? "Un"
                }).ToList() ?? new List<RecetaInsumoDto>(),

                PasosElaboracion = receta.PasosElaboracion?.OrderBy(p => p.Orden).Select(p => new PasosElaboracionDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Temperatura = p.Temperatura,
                    Tiempo = p.Tiempo,
                    Orden = p.Orden
                }).ToList() ?? new List<PasosElaboracionDto>()
            };
        }
        public async Task<int> DuplicarRecetaAsync(int idOriginal)
        {
            var original = await _recetaRepository.GetByIdAsync(idOriginal);                     
            if (original == null) return 0;
            string nombreOriginal = original.Nombre;//La primera version
            string nuevoNombre = "";
         
            string prefijoBusqueda = nombreOriginal.Contains(" V") ? $"{nombreOriginal}." : $"{nombreOriginal} V";

            var descendientes = await _recetaRepository.GetAllAsync();
            var hijosDirectos = descendientes
                .Where(r => r.Nombre.StartsWith(prefijoBusqueda))
                .ToList();
            int siguienteNumero = hijosDirectos.Count + 1;

            if (nombreOriginal.Contains(" V"))
            {
                nuevoNombre = $"{nombreOriginal}.{siguienteNumero}"; // Crea V2.1, V2.2...
            }
            else
            {
                nuevoNombre = $"{nombreOriginal} V{siguienteNumero + 1}"; // Crea V2, V3... (el +1 es porque la V1 es la original)
            }
            // Creamos la nueva receta, la copia
            var nuevaReceta = new Receta
            {
                Nombre = nuevoNombre, //Para que sea V2;V3;..
                Estilo = original.Estilo,
                BatchSizeLitros = original.BatchSizeLitros,
                Notas = original.Notas,
                Estado = EstadoReceta.Activa,
                // Clonamos la lista de Insumos
                // Creamos objetos nuevos para que tengan su propio ID en la tabla intermedia
                RecetaInsumos = original.RecetaInsumos.Select(i => new RecetaInsumo
                {
                    InsumoId = i.InsumoId,
                    Cantidad = i.Cantidad,
                    unidadMedidaId = i.unidadMedidaId
                }).ToList(),

                // 4. Clonamos la lista de Pasos
                PasosElaboracion = original.PasosElaboracion.Select(p => new PasosElaboracion
                {
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Temperatura = p.Temperatura,
                    Tiempo = p.Tiempo,
                    Orden = p.Orden,
                }).ToList()
            };

            // 5. Guardamos la nueva receta en la base de datos
            await _recetaRepository.AddAsync(nuevaReceta);
            return nuevaReceta.IdReceta;
        }

        //INSUMOS EN RECETA//
        private async Task<bool> ExisteNombreAsync(string nombre, int? idExcluido = null)
        {
            var todas = await _recetaRepository.GetAllAsync();
            var coincidencia = todas.Where(r => r.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));

            if (idExcluido.HasValue)
                return coincidencia.Any(r => r.IdReceta != idExcluido.Value);

            return coincidencia.Any();
        }

        public async Task<bool> AddInsumoToReceta(int id, RecetaInsumoDto dto)
        {
            var receta = await _recetaRepository.GetByIdAsync(id);
            if (receta == null) return false;         
         
            if (receta.RecetaInsumos.Any(x => x.InsumoId == dto.InsumoId))
            {               
                throw new Exception("Este insumo ya está cargado en la receta. Use el botón editar si desea cambiar la cantidad.");
            }     
            
             var nuevaRelacion = new RecetaInsumo
                {
                    RecetaId = id,
                    InsumoId = dto.InsumoId,
                    Cantidad = dto.Cantidad,
                    unidadMedidaId = dto.UnidadMedidaId
                };

                return await _recetaRepository.AddInsumoAsync(nuevaRelacion);             
        }


        public async Task<bool> RemoveInsumoDeReceta(int id, int insumoId)
        {
            return await _recetaRepository.RemoveInsumoAsync(id, insumoId);
        }

        public async Task<bool> ActualizarInsumoEnRecetaAsync(int recetaId, RecetaInsumoDto dto, bool Suma)
        {
            var receta = await _recetaRepository.GetByIdAsync(recetaId);
            if (receta == null) return false;

            // Relación existente entre la receta y el insumo para actualizar
            var existente = receta.RecetaInsumos.FirstOrDefault(x => x.InsumoId == dto.InsumoId);
            if (existente == null) return false;
            if (Suma)
            {
                existente.Cantidad += dto.Cantidad; // SUMA: (Cantidad actual + lo ingresado)
            }
            else
            {
                existente.Cantidad = dto.Cantidad;  // REEMPLAZA: (Pisa el valor con lo nuevo)
            }
            existente.unidadMedidaId = dto.UnidadMedidaId;            
            return await _recetaRepository.UpdateAsync(receta);
        }

        //PASOS DE ELABORACIÓN//
        public async Task<PasosElaboracion> CrearPasoAsync(int recetaId, PasosElaboracion paso)
        {
            var recetaExistente = await _recetaRepository.GetByIdAsync(recetaId);
            if (recetaExistente == null) throw new Exception("La receta no existe.");
            //si el usuario pone un orden que ya existe desplazo los demas pasos para que no queden con el mismo orden
            var pasosExistentes = recetaExistente.PasosElaboracion?.Where(p => p.Orden >= paso.Orden)
                .OrderByDescending(p => p.Orden);//Ordeno de mayor a menor para que al desplazar los pasos no se pisen entre sí
            foreach (var p in pasosExistentes)
            {
                p.Orden++;
                await _recetaRepository.UpdatePasoAsync(p);
            }
            paso.RecetaId = recetaId;
            return await _recetaRepository.AddPasoAsync(paso);
        }

        public async Task<bool> EditarPasoAsync(int recetaId, int pasoId, PasosElaboracion pasoEditado)
        {
            var receta = await _recetaRepository.GetByIdAsync(recetaId);
            if (receta == null || receta.PasosElaboracion == null) return false;

            var pasoExistente = receta.PasosElaboracion.FirstOrDefault(p => p.Id == pasoId);
            if (pasoExistente == null) return false;
            if (pasoExistente.Orden != pasoEditado.Orden)
            {
                var pasosA_Mover = receta.PasosElaboracion
                    .Where(p => p.Id != pasoId && p.Orden >= pasoEditado.Orden)
                    .OrderByDescending(p => p.Orden);

                foreach (var p in pasosA_Mover)
                {
                    p.Orden++;
                    await _recetaRepository.UpdatePasoAsync(p);
                }
            }
            pasoExistente.Nombre = pasoEditado.Nombre;
            pasoExistente.Descripcion = pasoEditado.Descripcion;
            pasoExistente.Temperatura = pasoEditado.Temperatura;
            pasoExistente.Tiempo = pasoEditado.Tiempo;
            pasoExistente.Orden = pasoEditado.Orden;

            return await _recetaRepository.UpdatePasoAsync(pasoExistente);
        }

        public async Task<bool> EliminarPasoAsync(int recetaId, int pasoId)
        {
            // 1. Borro el paso
            var eliminado = await _recetaRepository.DeletePasoAsync(pasoId);
            if (!eliminado) return false;

            // 2. Busco la receta para obtener los pasos que quedaron
            var receta = await _recetaRepository.GetByIdAsync(recetaId);
            if (receta != null && receta.PasosElaboracion != null)
            {
                // Agrupo los pasos restantes ordenados por su orden actual
                var pasosRestantes = receta.PasosElaboracion.OrderBy(p => p.Orden).ToList();
                // Se re asigna el orden de cada paso para que queden consecutivos (1, 2, 3, ...)
                for (int i = 0; i < pasosRestantes.Count; i++)
                {
                    pasosRestantes[i].Orden = i + 1;
                    await _recetaRepository.UpdatePasoAsync(pasosRestantes[i]);
                }
            }
            return true;
        }
    }
}