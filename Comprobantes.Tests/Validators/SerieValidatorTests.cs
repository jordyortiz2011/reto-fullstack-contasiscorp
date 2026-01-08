using Comprobantes.Application.Commands.CrearComprobante;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Comprobantes.Tests.Validators;

public class SerieValidatorTests
{
    private readonly CrearComprobanteCommandValidator _validator;

    public SerieValidatorTests()
    {
        _validator = new CrearComprobanteCommandValidator();
    }

    [Theory]
    [InlineData("F001")]
    [InlineData("F002")]
    [InlineData("F999")]
    public void Factura_FormatoSerieValido_NoDebeGenerarError(string serie)
    {
        // Arrange
        var command = new CrearComprobanteCommand
        {
            Tipo = "Factura",
            Serie = serie,
            RucEmisor = "20123456789",
            RazonSocialEmisor = "Test",
            RucReceptor = "20987654321",
            RazonSocialReceptor = "Cliente",
            Items = new List<CrearComprobanteItemCommand>
            {
                new() { Descripcion = "Item 1", Cantidad = 1, PrecioUnitario = 100 }
            }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Serie);
    }

    [Theory]
    [InlineData("B001")]
    [InlineData("B002")]
    [InlineData("B999")]
    public void Boleta_FormatoSerieValido_NoDebeGenerarError(string serie)
    {
        // Arrange
        var command = new CrearComprobanteCommand
        {
            Tipo = "Boleta",
            Serie = serie,
            RucEmisor = "20123456789",
            RazonSocialEmisor = "Test",
            Items = new List<CrearComprobanteItemCommand>
            {
                new() { Descripcion = "Item 1", Cantidad = 1, PrecioUnitario = 100 }
            }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Serie);
    }

    [Theory]
    [InlineData("B001")] // Serie de Boleta en Factura
    [InlineData("A001")] // Letra incorrecta
    [InlineData("F01")]  // Muy corta
    [InlineData("F0001")] // Muy larga
    public void Factura_FormatoSerieInvalido_DebeGenerarError(string serie)
    {
        // Arrange
        var command = new CrearComprobanteCommand
        {
            Tipo = "Factura",
            Serie = serie,
            RucEmisor = "20123456789",
            RazonSocialEmisor = "Test",
            RucReceptor = "20987654321",
            RazonSocialReceptor = "Cliente",
            Items = new List<CrearComprobanteItemCommand>
            {
                new() { Descripcion = "Item 1", Cantidad = 1, PrecioUnitario = 100 }
            }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Serie);
    }
}