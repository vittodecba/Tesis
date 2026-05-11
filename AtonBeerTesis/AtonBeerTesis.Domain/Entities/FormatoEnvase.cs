namespace AtonBeerTesis.Domain.Entities
{
    public class FormatoEnvase
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal CapacidadLitros { get; set; }
        public bool EsRetornable { get; set; } = false;
        public ICollection<ProductoStock> Productos { get; set; } = new List<ProductoStock>();
        public ICollection<LoteDesignacion> Designaciones { get; set; } = new List<LoteDesignacion>();
        public ICollection<Barril> Barriles { get; set; } = new List<Barril>();
    }
}
