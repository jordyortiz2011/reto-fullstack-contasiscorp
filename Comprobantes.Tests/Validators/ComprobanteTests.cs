using Comprobantes.Domain.Entities;
using Comprobantes.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Comprobantes.Tests.Domain;

public class ComprobanteTests
{
    [Fact]
    public void Comprobante_DebeCalcularIGVCorrectamente()
    {
        // Arrange
        var items = new List<ComprobanteItem>
        {
            new ComprobanteItem("Item 1", 1, 1000m),
            new ComprobanteItem("Item 2", 2.5m, 150m)
        };

        // Act
        var comprobante = new Comprobante(
            TipoComprobante.Factura,
            "F001",
            1,
            "20123456789",
            "Empresa",
            "20987654321",
            "Cliente",
            items
        );

        // Assert
        comprobante.SubTotal.Should().Be(1375m); // 1000 + 375
        comprobante.IGV.Should().Be(247.50m); // 1375 * 0.18
        comprobante.Total.Should().Be(1622.50m); // 1375 + 247.50
    }

    [Fact]
    public void Comprobante_DebeCalcularSubtotalCorrectamente()
    {
        // Arrange
        var items = new List<ComprobanteItem>
        {
            new ComprobanteItem("Item 1", 2, 50m),
            new ComprobanteItem("Item 2", 3, 30m)
        };

        // Act
        var comprobante = new Comprobante(
            TipoComprobante.Boleta,
            "B001",
            1,
            "20123456789",
            "Empresa",
            null,
            null,
            items
        );

        // Assert
        comprobante.SubTotal.Should().Be(190m); // (2*50) + (3*30) = 100 + 90
        comprobante.IGV.Should().Be(34.20m); // 190 * 0.18
        comprobante.Total.Should().Be(224.20m);
    }

    [Fact]
    public void Comprobante_InicialmenteDebeEstarEmitido()
    {
        // Arrange & Act
        var items = new List<ComprobanteItem>
        {
            new ComprobanteItem("Item 1", 1, 100m)
        };

        var comprobante = new Comprobante(
            TipoComprobante.Factura,
            "F001",
            1,
            "20123456789",
            "Empresa",
            "20987654321",
            "Cliente",
            items
        );

        // Assert
        comprobante.Estado.Should().Be(EstadoComprobante.Emitido);
    }

    [Fact]
    public void Anular_DebeMarcarComprobanteComoAnulado()
    {
        // Arrange
        var items = new List<ComprobanteItem>
        {
            new ComprobanteItem("Item 1", 1, 100m)
        };

        var comprobante = new Comprobante(
            TipoComprobante.Factura,
            "F001",
            1,
            "20123456789",
            "Empresa",
            "20987654321",
            "Cliente",
            items
        );

        // Act
        comprobante.Anular();

        // Assert
        comprobante.Estado.Should().Be(EstadoComprobante.Anulado);
    }

    [Fact]
    public void Anular_ComprobanteYaAnulado_DebeLanzarExcepcion()
    {
        // Arrange
        var items = new List<ComprobanteItem>
        {
            new ComprobanteItem("Item 1", 1, 100m)
        };

        var comprobante = new Comprobante(
            TipoComprobante.Factura,
            "F001",
            1,
            "20123456789",
            "Empresa",
            "20987654321",
            "Cliente",
            items
        );

        comprobante.Anular();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => comprobante.Anular());
        exception.Message.Should().Be("El comprobante ya está anulado");
    }
}