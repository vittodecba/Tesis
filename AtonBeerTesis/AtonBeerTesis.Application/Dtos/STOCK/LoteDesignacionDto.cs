namespace AtonBeerTesis.Application.Dtos.STOCK
{
    public class LoteDesignacionDto
    {
        public int Id { get; set; }
        public int FormatoEnvaseId { get; set; }
        public string NombreFormato { get; set; } = string.Empty;
        public decimal CapacidadLitros { get; set; }
        public decimal VolumenAsignado { get; set; }
        public decimal UnidadesResultantes => CapacidadLitros > 0 ? VolumenAsignado / CapacidadLitros : 0;
    }

    public class CreateLoteDesignacionDto
    {
        public int FormatoEnvaseId { get; set; }
        public decimal VolumenAsignado { get; set; }
    }
}
