namespace AtonBeerTesis.Application.Dtos.Cliente
{
    public class PatchClienteDto
    {
        public string? RazonSocial { get; set; }
        public string? TipoCliente { get; set; }
        public string? EstadoCliente { get; set; }

        public string? Email { get; set; }
        public string? Ubicacion { get; set; }

        public string? ContactoNombre { get; set; }
        public string? ContactoTelefono { get; set; }
        public string? ContactoEmail { get; set; }

        // Recomendación: NO permitir cambiar CUIT por PATCH.
        // Si igual querés permitirlo, agregá:
        // public string? Cuit { get; set; }
    }
}
