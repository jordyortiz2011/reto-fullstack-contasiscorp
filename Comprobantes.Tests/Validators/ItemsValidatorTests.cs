using Comprobantes.Application.Commands.CrearComprobante;
using FluentValidation.TestHelper;
using Xunit;

namespace Comprobantes.Tests.Validators;

public class ItemsValidatorTests
{
    private readonly CrearComprobanteCommandValidator _validator;

    public ItemsValidatorTests()
    {
        _validator = new CrearComprobanteCommandValidator();
    }

    [Fact]
    public void Items_DebeContenerAlMenosUno()
    {
        // Arrange
        var command = new CrearComprobanteCommand
        {
            Tipo = "Factura",
            Serie = "F001",
            RucEmisor = "20123456789",
            RazonSocialEmisor = "Test",
            RucReceptor = "20987654321",
            RazonSocialReceptor = "Cliente",
            Items = new List<CrearComprobanteItemCommand>() // Lista vacía
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Item_CantidadDebeSerMayorACero()
    {
        // Arrange
        var command = new CrearComprobanteCommand
        {
            Tipo = "Factura",
            Serie = "F001",
            RucEmisor = "20123456789",
            RazonSocialEmisor = "Test",
            RucReceptor = "20987654321",
            RazonSocialReceptor = "Cliente",
            Items = new List<CrearComprobanteItemCommand>
            {
                new() { Descripcion = "Item 1", Cantidad = 0, PrecioUnitario = 100 }
            }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].Cantidad");
    }

    [Fact]
    public void Item_PrecioUnitarioDebeSerMayorACero()
    {
        // Arrange
        var command = new CrearComprobanteCommand
        {
            Tipo = "Factura",
            Serie = "F001",
            RucEmisor = "20123456789",
            RazonSocialEmisor = "Test",
            RucReceptor = "20987654321",
            RazonSocialReceptor = "Cliente",
            Items = new List<CrearComprobanteItemCommand>
            {
                new() { Descripcion = "Item 1", Cantidad = 1, PrecioUnitario = 0 }
            }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].PrecioUnitario");
    }
}