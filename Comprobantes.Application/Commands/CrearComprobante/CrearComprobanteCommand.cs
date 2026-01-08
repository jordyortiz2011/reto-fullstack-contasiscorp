using Comprobantes.Application.DTOs;
using MediatR;

namespace Comprobantes.Application.Commands.CrearComprobante;

public class CrearComprobanteCommand : IRequest<ComprobanteDto>
{
    public string Tipo { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocialEmisor { get; set; } = string.Empty;
    public string? RucReceptor { get; set; }
    public string? RazonSocialReceptor { get; set; }
    public List<CrearComprobanteItemCommand> Items { get; set; } = new();
}

public class CrearComprobanteItemCommand
{
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}