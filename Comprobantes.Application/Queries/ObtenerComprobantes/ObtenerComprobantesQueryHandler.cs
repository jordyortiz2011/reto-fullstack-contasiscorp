using Comprobantes.Application.DTOs;
using Comprobantes.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comprobantes.Application.Queries.ObtenerComprobantes;

public class ObtenerComprobantesQueryHandler : IRequestHandler<ObtenerComprobantesQuery, PagedResult<ComprobanteDto>>
{
    private readonly IComprobanteRepository _repository;
    private readonly ILogger<ObtenerComprobantesQueryHandler> _logger;

    public ObtenerComprobantesQueryHandler(
        IComprobanteRepository repository,
        ILogger<ObtenerComprobantesQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResult<ComprobanteDto>> Handle(ObtenerComprobantesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Obteniendo comprobantes - Página: {Page}, Tamaño: {PageSize}, Tipo: {Tipo}, Estado: {Estado}",
            request.Page,
            request.PageSize,
            request.Tipo,
            request.Estado);

        var (items, totalCount) = await _repository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.FechaDesde,
            request.FechaHasta,
            request.Tipo,
            request.RucReceptor,
            request.Estado);

        var dtos = items.Select(comprobante => new ComprobanteDto
        {
            Id = comprobante.Id,
            Tipo = comprobante.Tipo.ToString(),
            Serie = comprobante.Serie,
            Numero = comprobante.Numero,
            FechaEmision = comprobante.FechaEmision,
            RucEmisor = comprobante.RucEmisor,
            RazonSocialEmisor = comprobante.RazonSocialEmisor,
            RucReceptor = comprobante.RucReceptor,
            RazonSocialReceptor = comprobante.RazonSocialReceptor,
            SubTotal = comprobante.SubTotal,
            IGV = comprobante.IGV,
            Total = comprobante.Total,
            Estado = comprobante.Estado.ToString(),
            Items = comprobante.Items.Select(i => new ComprobanteItemDto
            {
                Descripcion = i.Descripcion,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario,
                Subtotal = i.Subtotal
            }).ToList()
        }).ToList();

        return new PagedResult<ComprobanteDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}