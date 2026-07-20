using AtonBeerTesis.Application.DTOs;
using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Domain.Entities;
using AtonBeerTesis.Domain.Enums;
using AtonBeerTesis.Domain.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AtonBeerTesis.Application.Services
{
    public class LoteService : ILoteService
    {
        private readonly ILoteRepository _repository;
        private readonly IFermentadorRepository _fermentadorRepository;
        private readonly IPlanificacionRepository _planificacionRepository;
        private readonly IRepository<ProductoStock> _productoStockRepository;
        private readonly IRepository<MovimientoStock> _movimientoStockRepository;
        private readonly IBarrilRepository _barrilRepository;

        public LoteService(
            ILoteRepository repository,
            IFermentadorRepository fermentadorRepository,
            IPlanificacionRepository planificacionRepository,
            IRepository<ProductoStock> productoStockRepository,
            IRepository<MovimientoStock> movimientoStockRepository,
            IBarrilRepository barrilRepository)
        {
            _repository = repository;
            _fermentadorRepository = fermentadorRepository;
            _planificacionRepository = planificacionRepository;
            _productoStockRepository = productoStockRepository;
            _movimientoStockRepository = movimientoStockRepository;
            _barrilRepository = barrilRepository;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<List<LoteDto>> GetAllAsync()
        {
            var lotes = await _repository.GetAllAsync();

            return lotes.Select(l => new LoteDto
            {
                Id = l.Id,
                Codigo = l.Codigo,
                VolumenLitros = l.VolumenLitros,
                RecetaId = l.RecetaId,
                RecetaNombre = l.Receta?.Nombre,
                FermentadorId = l.FermentadorId,
                FermentadorNombre = l.Fermentador?.Nombre,
                FechaElaboracion = l.FechaElaboracion,
                Estilo = l.Estilo ?? l.Receta?.Estilo,
                Inoculo = l.Inoculo,
                Responsable = l.Responsable,
                DiasEstimadosFermentacion = l.DiasEstimadosFermentacion,
                Estado = l.Estado.ToString(),
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
                Estilo = lote.Estilo ?? lote.Receta?.Estilo,
                Inoculo = lote.Inoculo,
                Responsable = lote.Responsable,
                DiasEstimadosFermentacion = lote.DiasEstimadosFermentacion,
                Estado = lote.Estado.ToString(),
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
                VolumenLitros = lote.VolumenLitros,
                RecetaId = lote.RecetaId,
                RecetaNombre = lote.Receta?.Nombre,
                FermentadorId = lote.FermentadorId,
                FermentadorNombre = lote.Fermentador?.Nombre,
                FechaElaboracion = lote.FechaElaboracion,
                Estilo = lote.Estilo ?? lote.Receta?.Estilo,
                Inoculo = lote.Inoculo,
                Responsable = lote.Responsable,
                DiasEstimadosFermentacion = lote.DiasEstimadosFermentacion,
                Estado = lote.Estado.ToString(),
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

            var lote = new Lote
            {
                Codigo = dto.Codigo,
                RecetaId = dto.RecetaId ?? 0,
                FermentadorId = dto.FermentadorId,
                FechaElaboracion = dto.FechaElaboracion,
                FechaCreacion = DateTime.Now,
                Estilo = dto.Estilo,
                Inoculo = dto.Inoculo,
                Responsable = dto.Responsable,
                DiasEstimadosFermentacion = dto.DiasEstimadosFermentacion,
                Observaciones = dto.Observaciones,
                Estado = EstadoLote.EnProceso
            };

            await _repository.CreateAsync(lote);

            fermentador.Estado = EstadoFermentador.Ocupado;
            await _fermentadorRepository.UpdateAsync(fermentador);

            return new LoteDto
            {
                Id = lote.Id,
                Codigo = lote.Codigo,
                RecetaId = lote.RecetaId,
                RecetaNombre = lote.Receta?.Nombre,
                FermentadorId = lote.FermentadorId,
                FermentadorNombre = fermentador.Nombre,
                FechaElaboracion = lote.FechaElaboracion,
                Estilo = lote.Estilo,
                Inoculo = lote.Inoculo,
                Responsable = lote.Responsable,
                DiasEstimadosFermentacion = lote.DiasEstimadosFermentacion,
                Estado = lote.Estado.ToString(),
                Observaciones = lote.Observaciones,
                FechaFinReal = lote.FechaFinReal
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateLoteDto dto)
        {
            var lote = await _repository.GetByIdAsync(id);
            if (lote == null) return false;

            // Guard: un lote ya cerrado no se puede editar ni revertir a un estado activo.
            if (lote.Estado == EstadoLote.Finalizado || lote.Estado == EstadoLote.Descartado)
                throw new InvalidOperationException($"El lote ya está {lote.Estado} y no admite cambios.");

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
            {
                var nuevoEstado = Enum.Parse<EstadoLote>(dto.Estado);

                // Un fermentador solo puede tener UN lote EnProceso a la vez: si este lote arranca
                // (pasa a EnProceso) y su fermentador ya tiene otro lote en proceso, se rechaza.
                if (nuevoEstado == EstadoLote.EnProceso
                    && lote.Estado != EstadoLote.EnProceso
                    && lote.FermentadorId.HasValue)
                {
                    var loteActivo = await _repository.GetActivoByFermentadorIdAsync(lote.FermentadorId.Value);
                    if (loteActivo != null && loteActivo.Id != lote.Id)
                        throw new Exception("Ese fermentador ya tiene un lote en proceso.");

                    // Respetar el ORDEN de planificación: si el lote tiene una reserva anterior
                    // pendiente en el mismo fermentador, no puede arrancar hasta que esa se cierre.
                    var plan = await _planificacionRepository.GetByLoteIdAsync(lote.Id);
                    if (plan != null)
                    {
                        var reservaAnterior = await _planificacionRepository.GetReservaAnteriorPendienteAsync(
                            lote.FermentadorId.Value, plan.FechaInicio, plan.Id);
                        if (reservaAnterior != null)
                            throw new Exception(
                                $"Debe respetar el orden de planificación: primero finalice, descarte o elimine el lote " +
                                $"'{reservaAnterior.Lote?.Codigo}' antes de iniciar este.");
                    }
                }

                lote.Estado = nuevoEstado;
            }

            return await _repository.UpdateAsync(lote);
        }

        public async Task<bool> FinalizarAsync(int id, EstadoLote estadoFinal = EstadoLote.Finalizado)
        {
            if (estadoFinal != EstadoLote.Finalizado && estadoFinal != EstadoLote.Descartado)
                throw new InvalidOperationException("Estado inválido para finalización. Use Finalizado o Descartado.");

            var lote = await _repository.GetByIdAsync(id);
            if (lote == null) return false;

            // Guard: un lote ya cerrado no se puede volver a finalizar/descartar (evita, entre
            // otras cosas, duplicar la generación de stock).
            if (lote.Estado == EstadoLote.Finalizado || lote.Estado == EstadoLote.Descartado)
                throw new InvalidOperationException($"El lote ya está {lote.Estado} y no admite cambios de estado.");

            // ── VALIDACIONES PREVIAS (antes de mutar nada) ──────────────────────────────
            // Solo al Finalizar. Si alguna falla, salimos por excepción SIN haber tocado el
            // lote, el fermentador ni la planificación. Antes estas validaciones corrían
            // después de mutar el estado, lo que dejaba lotes "Finalizado" sin stock y
            // fermentadores trabados en Sucio cuando la designación/barriles no daban.
            var barrilesReservados = new Dictionary<int, List<Barril>>();
            if (estadoFinal == EstadoLote.Finalizado)
            {
                // a) La designación de volumen debe cubrir todo el volumen del lote.
                var volumenDesignado = lote.Designaciones.Sum(d => d.VolumenAsignado);
                if (volumenDesignado < (decimal)lote.VolumenLitros)
                    throw new InvalidOperationException(
                        $"No se puede finalizar: {volumenDesignado}L designados de {lote.VolumenLitros}L totales. " +
                        "Completá la designación de volumen en el detalle del lote.");

                // b) Pre-validación de barriles: formatosRetornables = { formatoId -> capacidadLitros }.
                var formatosRetornables = await _barrilRepository.ObtenerFormatosRetornablesAsync();
                foreach (var des in lote.Designaciones)
                {
                    if (!formatosRetornables.TryGetValue(des.FormatoEnvaseId, out var capacidad))
                        continue; // formato no retornable, sin barriles físicos

                    int unidadesRequeridas = (int)(des.VolumenAsignado / capacidad);
                    var disponibles = await _barrilRepository.GetDisponiblesAsync(des.FormatoEnvaseId, unidadesRequeridas);

                    if (disponibles.Count < unidadesRequeridas)
                        throw new InvalidOperationException(
                            $"No hay barriles suficientes para el formato {des.FormatoEnvaseId}: " +
                            $"se necesitan {unidadesRequeridas} y solo hay {disponibles.Count} disponible(s). " +
                            "Registrá más barriles en el módulo de Barriles antes de finalizar.");

                    barrilesReservados[des.FormatoEnvaseId] = disponibles;
                }
            }

            // ── A partir de acá ya está todo validado: recién ahora mutamos ─────────────

            // 1. Cerrar el lote con el estado indicado
            lote.Estado = estadoFinal;
            lote.FechaFinReal = DateTime.Now;

            var updated = await _repository.UpdateAsync(lote);
            if (!updated) return false;

            // 2. Marcar fermentador como Sucio (en ambos casos: Finalizado y Descartado)
            var fermentador = lote.FermentadorId.HasValue
                ? await _fermentadorRepository.GetByIdAsync(lote.FermentadorId.Value)
                : null;
            if (fermentador != null)
            {
                fermentador.Estado = EstadoFermentador.Sucio;
                await _fermentadorRepository.UpdateAsync(fermentador);
            }

            // 3. Sincronizar la planificación asociada al lote
            var planificacion = await _planificacionRepository.GetByLoteIdAsync(lote.Id);
            if (planificacion != null)
            {
                planificacion.Estado = estadoFinal;
                await _planificacionRepository.UpdateAsync(planificacion);
            }

            // 4. Generar ingresos de stock + llenar barriles (solo si finaliza, no si descarta)
            if (estadoFinal == EstadoLote.Finalizado && lote.Designaciones.Any())
            {
                var estiloLote = lote.Estilo ?? lote.Receta?.Estilo ?? string.Empty;
                var todosLosProductos = await _productoStockRepository.FindAllAsync();

                foreach (var designacion in lote.Designaciones)
                {
                    var formato = designacion.FormatoEnvase;
                    if (formato == null || formato.CapacidadLitros <= 0) continue;

                    var unidades = designacion.VolumenAsignado / formato.CapacidadLitros;

                    var productoStock = todosLosProductos.FirstOrDefault(p =>
                        p.FormatoEnvaseId == designacion.FormatoEnvaseId &&
                        p.Estilo.Equals(estiloLote, StringComparison.OrdinalIgnoreCase) &&
                        p.RecetaId == lote.RecetaId);

                    // Si no existe, lo creamos on-the-fly
                    if (productoStock == null && !string.IsNullOrWhiteSpace(estiloLote))
                    {
                        productoStock = new ProductoStock
                        {
                            FormatoEnvaseId = designacion.FormatoEnvaseId,
                            Estilo = estiloLote,
                            RecetaId = lote.RecetaId,
                            StockActual = 0
                        };
                        await _productoStockRepository.AddAsync(productoStock);
                        todosLosProductos.Add(productoStock);
                    }

                    if (productoStock == null) continue;

                    var stockPrevio = productoStock.StockActual;
                    productoStock.StockActual += unidades;
                    await _movimientoStockRepository.AddAsync(new MovimientoStock
                    {
                        ProductoStockId = productoStock.Id,
                        LoteId = lote.Id,
                        Cantidad = unidades,
                        TipoMovimiento = "Ingreso",
                        MotivoMovimiento = "Produccion",
                        StockPrevio = stockPrevio,
                        StockResultante = productoStock.StockActual,
                        Fecha = DateTime.Now
                    });
                }

                // 6. LLENAR BARRILES — ExecuteUpdateAsync para bypass del change tracker
                var idsALlenar = barrilesReservados.Values
                    .SelectMany(lista => lista.Select(b => b.Id))
                    .ToList();

                if (idsALlenar.Any())
                    await _barrilRepository.MarcarComoLlenosAsync(idsALlenar, lote.Id);
            }

            return true;
        }

        // ── Reporte P2 · Cumplimiento de planificación ─────────────────────────────
        public async Task<ReporteCumplimientoDto> ObtenerReporteCumplimientoAsync(DateTime fechaDesde, DateTime fechaHasta)
        {
            var lotes = await _repository.GetFinalizadosEnRangoAsync(fechaDesde, fechaHasta);

            var detalle = lotes.Select(l =>
            {
                // Días reales = desde la elaboración hasta el cierre real, redondeado.
                // FechaFinReal está garantizada no-nula por el filtro del repositorio.
                var diasReales = (int)Math.Round((l.FechaFinReal!.Value - l.FechaElaboracion).TotalDays);
                return new LoteCumplimientoDto
                {
                    CodigoLote = l.Codigo,
                    Receta = l.Receta?.Nombre,
                    Estilo = l.Estilo ?? l.Receta?.Estilo,
                    DiasEstimados = l.DiasEstimadosFermentacion,
                    DiasReales = diasReales,
                    DesvioDias = diasReales - l.DiasEstimadosFermentacion,
                    Estado = l.Estado.ToString()
                };
            }).ToList();

            // Se muestran todos los lotes cerrados del rango tal cual estén. Un lote con
            // duración inválida (0 días o negativa) debe manejarse marcándolo Descartado a mano,
            // no ocultándolo automáticamente.
            var finalizados = detalle.Where(d => d.Estado == EstadoLote.Finalizado.ToString()).ToList();
            var descartados = detalle.Count(d => d.Estado == EstadoLote.Descartado.ToString());

            int finalizadosCount = finalizados.Count;
            int aTiempo = finalizados.Count(d => d.DiasReales <= d.DiasEstimados);
            int cerradosTotal = finalizadosCount + descartados;

            return new ReporteCumplimientoDto
            {
                LotesFinalizados = finalizadosCount,
                PorcentajeATiempo = finalizadosCount > 0
                    ? Math.Round((double)aTiempo / finalizadosCount * 100, 1)
                    : 0,
                DesvioPromedioDias = finalizadosCount > 0
                    ? Math.Round(finalizados.Average(d => d.DesvioDias), 1)
                    : 0,
                TasaDescarte = cerradosTotal > 0
                    ? Math.Round((double)descartados / cerradosTotal * 100, 1)
                    : 0,
                Detalle = detalle
            };
        }

        public async Task<byte[]> GenerarPdfReporteCumplimientoAsync(ReportePdfRequestDto request)
        {
            var data = await ObtenerReporteCumplimientoAsync(request.FechaDesde, request.FechaHasta);

            byte[]? imgPrincipal = null;
            if (!string.IsNullOrEmpty(request.GraficoPrincipalBase64))
            {
                var base64Data = request.GraficoPrincipalBase64.Contains(",")
                    ? request.GraficoPrincipalBase64.Split(',')[1]
                    : request.GraficoPrincipalBase64;
                imgPrincipal = Convert.FromBase64String(base64Data);
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("ATON BEER").FontSize(24).Black().FontColor("#4A2C2A");
                        col.Item().Text("Reporte de Cumplimiento de Planificación").FontSize(14).SemiBold().FontColor("#E67E22");
                        col.Item().Text($"Período: {request.FechaDesde:dd/MM/yyyy} al {request.FechaHasta:dd/MM/yyyy}").FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().Text($"Fecha de Emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(20);

                        // KPIs
                        col.Item().Background(Colors.Grey.Lighten4).Padding(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("LOTES FINALIZADOS").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                                c.Item().Text($"{data.LotesFinalizados}").FontSize(14).Black().FontColor("#3A2220");
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("FINALIZADOS A TIEMPO").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                                c.Item().Text($"{data.PorcentajeATiempo}%").FontSize(14).Black().FontColor("#22c55e");
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("DESVÍO PROMEDIO").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                                var signo = data.DesvioPromedioDias > 0 ? "+" : "";
                                c.Item().Text($"{signo}{data.DesvioPromedioDias} días").FontSize(14).Black().FontColor("#E67E22");
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("TASA DE DESCARTE").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                                c.Item().Text($"{data.TasaDescarte}%").FontSize(14).Black().FontColor("#ef4444");
                            });
                        });

                        // Gráfico Estimado vs Real
                        if (imgPrincipal != null)
                        {
                            col.Item().EnsureSpace().Column(c =>
                            {
                                c.Item().Text("Días estimados vs. reales por lote").FontSize(14).SemiBold().FontColor("#4A2C2A");
                                c.Item().PaddingTop(10).Image(imgPrincipal);
                            });
                        }

                        // Detalle por lote
                        col.Item().Text("Detalle por lote").FontSize(14).SemiBold().FontColor("#4A2C2A");
                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1.2f); // Lote
                                columns.RelativeColumn(2f);   // Receta
                                columns.RelativeColumn(1f);   // Días est.
                                columns.RelativeColumn(1f);   // Días reales
                                columns.RelativeColumn(1f);   // Desvío
                                columns.RelativeColumn(1.2f); // Estado
                            });

                            tabla.Header(header =>
                            {
                                void H(string t, bool right = false)
                                {
                                    var cell = header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5);
                                    (right ? cell.AlignRight() : cell).Text(t).SemiBold().FontSize(9).FontColor(Colors.Grey.Darken2);
                                }
                                H("Lote"); H("Receta"); H("Días Est.", true); H("Días Reales", true); H("Desvío", true); H("Estado");
                            });

                            foreach (var l in data.Detalle)
                            {
                                void Cell(string t, bool right = false, string? color = null)
                                {
                                    var cell = tabla.Cell().PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten4);
                                    var txt = (right ? cell.AlignRight() : cell).Text(t).FontSize(9);
                                    if (color != null) txt.FontColor(color);
                                }
                                var signo = l.DesvioDias > 0 ? "+" : "";
                                var estadoColor = l.Estado == EstadoLote.Finalizado.ToString() ? "#22c55e" : "#ef4444";
                                Cell(l.CodigoLote);
                                Cell(l.Receta ?? "—");
                                Cell($"{l.DiasEstimados}", true);
                                Cell($"{l.DiasReales}", true);
                                Cell($"{signo}{l.DesvioDias} d", true);
                                Cell(l.Estado, false, estadoColor);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}