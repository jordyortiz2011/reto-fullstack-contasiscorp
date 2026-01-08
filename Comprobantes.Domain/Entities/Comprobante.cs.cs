using Comprobantes.Domain.Enums;

namespace Comprobantes.Domain.Entities;

public class Comprobante
{
    public Guid Id { get; private set; }
    public TipoComprobante Tipo { get; private set; }
    public string Serie { get; private set; } = string.Empty;
    public int Numero { get; private set; }
    public DateTime FechaEmision { get; private set; }
    public string RucEmisor { get; private set; } = string.Empty;
    public string RazonSocialEmisor { get; private set; } = string.Empty;
    public string? RucReceptor { get; private set; }
    public string? RazonSocialReceptor { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal IGV { get; private set; }
    public decimal Total { get; private set; }
    public EstadoComprobante Estado { get; private set; }

    // Navigation property
    private readonly List<ComprobanteItem> _items = new();
    public IReadOnlyCollection<ComprobanteItem> Items => _items.AsReadOnly();

    // Constructor privado para EF Core
    private Comprobante() { }

    public Comprobante(
        TipoComprobante tipo,
        string serie,
        int numero,
        string rucEmisor,
        string razonSocialEmisor,
        string? rucReceptor,
        string? razonSocialReceptor,
        List<ComprobanteItem> items)
    {
        Id = Guid.NewGuid();
        Tipo = tipo;
        Serie = serie;
        Numero = numero;
        FechaEmision = DateTime.UtcNow;
        RucEmisor = rucEmisor;
        RazonSocialEmisor = razonSocialEmisor;
        RucReceptor = rucReceptor;
        RazonSocialReceptor = razonSocialReceptor;
        Estado = EstadoComprobante.Emitido;

        _items = items;
        CalcularTotales();
    }

    private void CalcularTotales()
    {
        SubTotal = _items.Sum(item => item.Subtotal);
        IGV = SubTotal * 0.18m;
        Total = SubTotal + IGV;
    }

    public void Anular()
    {
        if (Estado == EstadoComprobante.Anulado)
        {
            throw new InvalidOperationException("El comprobante ya está anulado");
        }

        Estado = EstadoComprobante.Anulado;
    }
}