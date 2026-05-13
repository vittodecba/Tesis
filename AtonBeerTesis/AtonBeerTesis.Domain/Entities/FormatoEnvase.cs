using System.Text.Json.Serialization;

namespace AtonBeerTesis.Domain.Entities
{
    public class FormatoEnvase
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal CapacidadLitros { get; set; }
        [JsonIgnore]
        public ICollection<ProductoStock> Productos { get; set; } = new List<ProductoStock>();
        [JsonIgnore]
        public ICollection<LoteDesignacion> Designaciones { get; set; } = new List<LoteDesignacion>();
    }
}