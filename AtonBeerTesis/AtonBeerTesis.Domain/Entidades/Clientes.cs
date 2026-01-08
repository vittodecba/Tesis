using AtonBeerTesis.Domain.Enums;


namespace AtonBeerTesis.Domain.Entities;

public class Cliente
{
    // Identificación
    public int IdCliente { get; set; }

    // Clasificación
    public TipoCliente Tipocliente { get; set; } = TipoCliente.Externo;
    public EstadoCliente EstadoCliente { get; set; } = EstadoCliente.Activo;

    // Datos del cliente
    public string RazonSocial { get; set; } = null!;
    public string Cuit { get; set; } = null!; // string por ceros/guiones

    // Email general del cliente (compras/administración)
    public string? Email { get; set; }

    // Ubicación ampliable
    public string Ubicacion { get; set; } = null!;

    // Contacto actual (único, editable)
    public string? ContactoNombre { get; set; }
    public string? ContactoTelefono { get; set; }
    public string? ContactoEmail { get; set; }

    // Métricas (las actualiza el sistema)
    public DateTime? UltimaCompra { get; set; }   // última venta
    public DateTime? UltimoPedido { get; set; }   // último pedido creado
    public int TotalPedidos { get; set; }
    

   
}
