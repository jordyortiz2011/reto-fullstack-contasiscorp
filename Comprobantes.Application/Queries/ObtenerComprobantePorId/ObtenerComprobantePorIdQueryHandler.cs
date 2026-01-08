using Comprobantes.Application.DTOs;
using Comprobantes.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comprobantes.Application.Queries.ObtenerComprobantePorId;

public class ObtenerComprobantePorIdQueryHandler : IRequestHandler<ObtenerComprobantePorIdQuery, ComprobanteDto?>
{
    private readonly IComprobanteRepository _repository;
    private readonly ILogger<ObtenerComprobantePorIdQueryHandler> _logger;

    public ObtenerComprobantePorIdQueryHandler(
        IComprobanteRepository repository,
        ILogger<ObtenerComprobantePorIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ComprobanteDto?> Handle(ObtenerComprobantePorIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obteniendo comprobante {ComprobanteId}", request.Id);

        var comprobante = await _repository.GetByIdAsync(request.Id);

        if (comprobante == null)
        {
            _logger.LogWarning("Comprobante {ComprobanteId} no encontrado", request.Id);
            return null;
        }

        return new ComprobanteDto
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
        };
    }
}