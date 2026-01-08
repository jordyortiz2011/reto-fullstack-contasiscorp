using Comprobantes.Application.DTOs;
using Comprobantes.Domain.Enums;
using MediatR;

namespace Comprobantes.Application.Queries.ObtenerComprobantes;

public class ObtenerComprobantesQuery : IRequest<PagedResult<ComprobanteDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public TipoComprobante? Tipo { get; set; }
    public string? RucReceptor { get; set; }
    public EstadoComprobante? Estado { get; set; }
}