using Comprobantes.Domain.Enums;

namespace Comprobantes.Application.DTOs;

public class ComprobanteDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public int Numero { get; set; }
    public DateTime FechaEmision { get; set; }
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocialEmisor { get; set; } = string.Empty;
    public string? RucReceptor { get; set; }
    public string? RazonSocialReceptor { get; set; }
    public decimal SubTotal { get; set; }
    public decimal IGV { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
    public List<ComprobanteItemDto> Items { get; set; } = new();
}