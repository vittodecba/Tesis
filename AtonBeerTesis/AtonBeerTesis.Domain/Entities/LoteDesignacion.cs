namespace AtonBeerTesis.Domain.Entities
{
    public class LoteDesignacion
    {
        public int Id { get; set; }
        public int LoteId { get; set; }
        public Lote Lote { get; set; } = null!;
        public int FormatoEnvaseId { get; set; }
        public FormatoEnvase FormatoEnvase { get; set; } = null!;
        public decimal VolumenAsignado { get; set; }
    }
}
