using MediatR;

namespace Comprobantes.Application.Commands.AnularComprobante;

public class AnularComprobanteCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public AnularComprobanteCommand(Guid id)
    {
        Id = id;
    }
}