using Comprobantes.Application.Commands.CrearComprobante;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Comprobantes.Tests.Validators;

public class CrearComprobanteCommandValidatorTests
{
    private readonly CrearComprobanteCommandValidator _validator;

    public CrearComprobanteCommandValidatorTests()
    {
        _validator = new CrearComprobanteCommandValidator();
    }

    [Fact]
    public void RucEmisor_DebeSerExactamente11Digitos()
    {
        // Arrange
        var command = new CrearComprobanteCommand
        {
            Tipo = "Factura",
            Serie = "F001",
            RucEmisor = "123456789", // 9 dígitos (inválido)
            RazonSocialEmisor = "Test",
            RucReceptor = "20123456789",
            RazonSocialReceptor = "Cliente",
            Items = new List<CrearComprobanteItemCommand>
            {
                new() { Descripcion = "Item 1", Cantidad = 1, PrecioUnitario = 100 }
            }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RucEmisor)
            .WithErrorMessage("El RUC debe tener exactamente 11 dígitos");
    }

    [Fact]
    public void RucEmisor_DebeTenerSoloDigitos()
    {
        // Arrange
        var command = new CrearComprobanteCommand
        {
            Tipo = "Factura",
            Serie = "F001",
            RucEmisor = "2012345678A", // Contiene letra (inválido)
            RazonSocialEmisor = "Test",
            RucReceptor = "20123456789",
            RazonSocialReceptor = "Cliente",
            Items = new List<CrearComprobanteItemCommand>
            {
                new() { Descripcion = "Item 1", Cantidad = 1, PrecioUnitario = 100 }
            }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RucEmisor)
            .WithErrorMessage("El RUC debe contener solo dígitos numéricos");
    }

    [Fact]
    public void RucEmisor_Valido_NoDebeGenerarError()
    {
        // Arrange
        var command = new CrearComprobanteCommand
        {
            Tipo = "Factura",
            Serie = "F001",
            RucEmisor = "20123456789", // 11 dígitos válidos
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
        result.ShouldNotHaveValidationErrorFor(x => x.RucEmisor);
    }
}