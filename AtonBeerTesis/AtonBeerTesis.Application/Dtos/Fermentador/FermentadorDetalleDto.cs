public class FermentadorDetalleDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Capacidad { get; set; }
    public string Estado { get; set; }
    public string Observaciones { get; set; }

    // Estos campos se llenan solo si hay una planificación "En Proceso"
    public int? LoteId { get; set; }
    public string? EstiloNombre { get; set; }
}