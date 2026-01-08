using Comprobantes.Application.Commands.AnularComprobante;
using Comprobantes.Application.Commands.CrearComprobante;
using Comprobantes.Application.DTOs;
using Comprobantes.Application.Queries.ObtenerComprobantePorId;
using Comprobantes.Application.Queries.ObtenerComprobantes;
using Comprobantes.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Comprobantes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ComprobantesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ComprobantesController> _logger;

    public ComprobantesController(IMediator mediator, ILogger<ComprobantesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene una lista paginada de comprobantes con filtros opcionales
    /// </summary>
    /// <param name="page">Número de página (default: 1)</param>
    /// <param name="pageSize">Tamaño de página (default: 10, máximo: 50)</param>
    /// <param name="fechaDesde">Fecha desde (formato: yyyy-MM-dd)</param>
    /// <param name="fechaHasta">Fecha hasta (formato: yyyy-MM-dd)</param>
    /// <param name="tipo">Tipo de comprobante (Factura o Boleta)</param>
    /// <param name="rucReceptor">RUC del receptor (11 dígitos)</param>
    /// <param name="estado">Estado del comprobante (Emitido o Anulado)</param>
    /// <returns>Lista paginada de comprobantes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ComprobanteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ComprobanteDto>>> GetComprobantes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] TipoComprobante? tipo = null,
        [FromQuery] string? rucReceptor = null,
        [FromQuery] EstadoComprobante? estado = null)
    {
        var query = new ObtenerComprobantesQuery
        {
            Page = page,
            PageSize = pageSize,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            Tipo = tipo,
            RucReceptor = rucReceptor,
            Estado = estado
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el detalle de un comprobante por su ID
    /// </summary>
    /// <param name="id">ID del comprobante (GUID)</param>
    /// <returns>Detalle del comprobante</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ComprobanteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ComprobanteDto>> GetComprobanteById(Guid id)
    {
        var query = new ObtenerComprobantePorIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Resource Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Comprobante con ID {id} no fue encontrado"
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Crea un nuevo comprobante electrónico
    /// </summary>
    /// <param name="command">Datos del comprobante a crear</param>
    /// <returns>Comprobante creado con todos los campos calculados</returns>
    /// <remarks>
    /// Ejemplo de request para crear una Factura:
    /// 
    ///     POST /api/comprobantes
    ///     {
    ///       "tipo": "Factura",
    ///       "serie": "F001",
    ///       "rucEmisor": "20123456789",
    ///       "razonSocialEmisor": "Mi Empresa S.A.C.",
    ///       "rucReceptor": "20987654321",
    ///       "razonSocialReceptor": "Cliente S.A.",
    ///       "items": [
    ///         {
    ///           "descripcion": "Servicio de consultoría",
    ///           "cantidad": 1,
    ///           "precioUnitario": 1000.00
    ///         },
    ///         {
    ///           "descripcion": "Horas adicionales",
    ///           "cantidad": 2.5,
    ///           "precioUnitario": 150.00
    ///         }
    ///       ]
    ///     }
    ///     
    /// Ejemplo de request para crear una Boleta (sin receptor):
    /// 
    ///     POST /api/comprobantes
    ///     {
    ///       "tipo": "Boleta",
    ///       "serie": "B001",
    ///       "rucEmisor": "20123456789",
    ///       "razonSocialEmisor": "Mi Empresa S.A.C.",
    ///       "items": [
    ///         {
    ///           "descripcion": "Producto A",
    ///           "cantidad": 3,
    ///           "precioUnitario": 25.50
    ///         }
    ///       ]
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ComprobanteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ComprobanteDto>> CreateComprobante([FromBody] CrearComprobanteCommand command)
    {
        _logger.LogInformation("Recibida solicitud para crear comprobante tipo {Tipo}, serie {Serie}", 
            command.Tipo, command.Serie);

        var result = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetComprobanteById),
            new { id = result.Id },
            result);
    }

    /// <summary>
    /// Anula un comprobante existente
    /// </summary>
    /// <param name="id">ID del comprobante a anular</param>
    /// <returns>Confirmación de anulación</returns>
    [HttpPut("{id}/anular")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AnularComprobante(Guid id)
    {
        _logger.LogInformation("Recibida solicitud para anular comprobante {ComprobanteId}", id);

        var command = new AnularComprobanteCommand(id);
        await _mediator.Send(command);

        return Ok(new
        {
            message = "Comprobante anulado exitosamente",
            comprobanteId = id
        });
    }
}