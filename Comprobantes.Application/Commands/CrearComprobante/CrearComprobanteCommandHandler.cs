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
        _logger.LogInformation("Creando comprobante tipo {Tipo}, serie {Serie}", request.Tipo, request.Serie);

        // Obtener el siguiente número para la serie
        var siguienteNumero = await _repository.GetNextNumeroAsync(request.Serie);

        // Convertir items
        var items = request.Items.Select(i => new ComprobanteItem(
            i.Descripcion,
            i.Cantidad,
            i.PrecioUnitario
        )).ToList();

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

        // Guardar
        await _repository.AddAsync(comprobante);

        _logger.LogInformation(
            "Comprobante creado exitosamente: {Id}, {Tipo} {Serie}-{Numero}", 
            comprobante.Id, 
            comprobante.Tipo, 
            comprobante.Serie, 
            comprobante.Numero);

        // Mapear a DTO
        return MapToDto(comprobante);
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