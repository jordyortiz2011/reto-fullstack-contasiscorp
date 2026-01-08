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
        _logger.LogInformation(
            "Iniciando anulación de comprobante - ComprobanteId: {ComprobanteId}",
            request.Id);

        try
        {
            var comprobante = await _repository.GetByIdAsync(request.Id);

            if (comprobante == null)
            {
                _logger.LogWarning(
                    "Comprobante no encontrado para anular - ComprobanteId: {ComprobanteId}",
                    request.Id);
                throw new ComprobanteNotFoundException(request.Id);
            }

            _logger.LogDebug(
                "Comprobante encontrado - Tipo: {Tipo}, Serie-Numero: {Serie}-{Numero}, Estado actual: {Estado}",
                comprobante.Tipo,
                comprobante.Serie,
                comprobante.Numero,
                comprobante.Estado);

            comprobante.Anular();
            await _repository.UpdateAsync(comprobante);

            _logger.LogInformation(
                "Comprobante anulado exitosamente - ID: {ComprobanteId}, Tipo: {Tipo}, Serie-Numero: {Serie}-{Numero}, RucEmisor: {RucEmisor}, RucReceptor: {RucReceptor}, Total: {Total}",
                comprobante.Id,
                comprobante.Tipo,
                comprobante.Serie,
                comprobante.Numero,
                comprobante.RucEmisor,
                comprobante.RucReceptor ?? "N/A",
                comprobante.Total);

            return true;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Intento de anular comprobante ya anulado - ComprobanteId: {ComprobanteId}",
                request.Id);
            throw;
        }
        catch (Exception ex) when (ex is not ComprobanteNotFoundException)
        {
            _logger.LogError(
                ex,
                "Error inesperado al anular comprobante - ComprobanteId: {ComprobanteId}",
                request.Id);
            throw;
        }
    }
}