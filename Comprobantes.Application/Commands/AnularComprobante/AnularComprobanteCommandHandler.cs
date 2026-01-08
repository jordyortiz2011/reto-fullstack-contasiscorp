using Comprobantes.Application.Interfaces;
using Comprobantes.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comprobantes.Application.Commands.AnularComprobante;

public class AnularComprobanteCommandHandler : IRequestHandler<AnularComprobanteCommand, bool>
{
    private readonly IComprobanteRepository _repository;
    private readonly ILogger<AnularComprobanteCommandHandler> _logger;

    public AnularComprobanteCommandHandler(
        IComprobanteRepository repository,
        ILogger<AnularComprobanteCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(AnularComprobanteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Anulando comprobante {ComprobanteId}", request.Id);

        var comprobante = await _repository.GetByIdAsync(request.Id);

        if (comprobante == null)
        {
            _logger.LogWarning("Comprobante {ComprobanteId} no encontrado", request.Id);
            throw new ComprobanteNotFoundException(request.Id);
        }

        try
        {
            comprobante.Anular();
            await _repository.UpdateAsync(comprobante);

            _logger.LogInformation(
                "Comprobante anulado exitosamente: {Id}, {Tipo} {Serie}-{Numero}",
                comprobante.Id,
                comprobante.Tipo,
                comprobante.Serie,
                comprobante.Numero);

            return true;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error al anular comprobante {ComprobanteId}", request.Id);
            throw;
        }
    }
}