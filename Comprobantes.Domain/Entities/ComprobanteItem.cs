namespace Comprobantes.Domain.Entities;

public class ComprobanteItem
{
    public Guid Id { get; private set; }
    public Guid ComprobanteId { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal Subtotal { get; private set; }

    // Navigation property
    public Comprobante Comprobante { get; private set; } = null!;

    // Constructor privado para EF Core
    private ComprobanteItem() { }

    public ComprobanteItem(string descripcion, decimal cantidad, decimal precioUnitario)
    {
        Id = Guid.NewGuid();
        Descripcion = descripcion;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        CalcularSubtotal();
    }

    private void CalcularSubtotal()
    {
        Subtotal = Cantidad * PrecioUnitario;
    }
}