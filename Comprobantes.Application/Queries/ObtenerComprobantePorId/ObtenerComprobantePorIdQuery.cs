using Comprobantes.Application.DTOs;
using MediatR;

namespace Comprobantes.Application.Queries.ObtenerComprobantePorId;

public class ObtenerComprobantePorIdQuery : IRequest<ComprobanteDto?>
{
    public Guid Id { get; set; }

    public ObtenerComprobantePorIdQuery(Guid id)
    {
        Id = id;
    }
}