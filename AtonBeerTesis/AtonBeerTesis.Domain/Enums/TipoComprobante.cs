namespace AtonBeerTesis.Domain.Enums
{
    // Tipo de comprobante (factura) según marco AFIP.
    // A = COD. 01 (IVA discriminado), B = COD. 06 (IVA incluido, no discriminado)
    public enum TipoComprobante
    {
        A = 1,
        B = 2
    }
}
