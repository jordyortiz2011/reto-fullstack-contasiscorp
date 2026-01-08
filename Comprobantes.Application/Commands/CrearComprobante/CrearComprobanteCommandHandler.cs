using Comprobantes.Application.DTOs;
using Comprobantes.Application.Interfaces;
using Comprobantes.Domain.Entities;
using Comprobantes.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comprobantes.Application.Commands.CrearComprobante;

public class CrearComprobanteCommandHandler : IRequestHandler<CrearComprobanteCommand, ComprobanteDto>
{
    private readonly IComprobanteRepository _repository;
    private readonly ILogger<CrearComprobanteCommandHandler> _logger;

    public CrearComprobanteCommandHandler(
        IComprobanteRepository repository,
        ILogger<CrearComprobanteCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ComprobanteDto> Handle(CrearComprobanteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Iniciando creación de comprobante - Tipo: {Tipo}, Serie: {Serie}, RucEmisor: {RucEmisor}, RucReceptor: {RucReceptor}",
            request.Tipo,
            request.Serie,
            request.RucEmisor,
            request.RucReceptor ?? "N/A");

        try
        {
            // Obtener el siguiente número para la serie
            var siguienteNumero = await _repository.GetNextNumeroAsync(request.Serie);

            _logger.LogDebug(
                "Número obtenido para serie {Serie}: {Numero}",
                request.Serie,
                siguienteNumero);

            // Convertir items
            var items = request.Items.Select(i => new ComprobanteItem(
                i.Descripcion,
                i.Cantidad,
                i.PrecioUnitario
            )).ToList();

            _logger.LogDebug(
                "Items procesados: {CantidadItems}",
                items.Count);

            // Crear el comprobante
            var tipo = Enum.Parse<TipoComprobante>(request.Tipo);
            var comprobante = new Comprobante(
                tipo,
                request.Serie,
                siguienteNumero,
                request.RucEmisor,
                request.RazonSocialEmisor,
                request.RucReceptor,
                request.RazonSocialReceptor,
                items
            );

            _logger.LogDebug(
                "Comprobante creado en memoria - SubTotal: {SubTotal}, IGV: {IGV}, Total: {Total}",
                comprobante.SubTotal,
                comprobante.IGV,
                comprobante.Total);

            // Guardar
            await _repository.AddAsync(comprobante);

            _logger.LogInformation(
                "Comprobante creado exitosamente - ID: {ComprobanteId}, Tipo: {Tipo}, Serie-Numero: {Serie}-{Numero}, Total: {Total}, RucEmisor: {RucEmisor}, RucReceptor: {RucReceptor}",
                comprobante.Id,
                comprobante.Tipo,
                comprobante.Serie,
                comprobante.Numero,
                comprobante.Total,
                comprobante.RucEmisor,
                comprobante.RucReceptor ?? "N/A");

            // Mapear a DTO
            return MapToDto(comprobante);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al crear comprobante - Tipo: {Tipo}, Serie: {Serie}, RucEmisor: {RucEmisor}",
                request.Tipo,
                request.Serie,
                request.RucEmisor);
            throw;
        }
    }

    private ComprobanteDto MapToDto(Comprobante comprobante)
    {
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