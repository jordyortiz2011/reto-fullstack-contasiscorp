using Comprobantes.Domain.Enums;
using FluentValidation;

namespace Comprobantes.Application.Commands.CrearComprobante;

public class CrearComprobanteCommandValidator : AbstractValidator<CrearComprobanteCommand>
{
    public CrearComprobanteCommandValidator()
    {
        RuleFor(x => x.Tipo)
            .NotEmpty()
            .WithMessage("El tipo de comprobante es obligatorio")
            .Must(tipo => tipo == "Factura" || tipo == "Boleta")
            .WithMessage("El tipo debe ser 'Factura' o 'Boleta'");

        RuleFor(x => x.Serie)
            .NotEmpty()
            .WithMessage("La serie es obligatoria")
            .Length(4)
            .WithMessage("La serie debe tener exactamente 4 caracteres")
            .Must((command, serie) => ValidarFormatoSerie(command.Tipo, serie))
            .WithMessage(command => command.Tipo == "Factura" 
                ? "El formato de serie para Factura debe ser F### (ej: F001)" 
                : "El formato de serie para Boleta debe ser B### (ej: B001)");

        RuleFor(x => x.RucEmisor)
            .NotEmpty()
            .WithMessage("El RUC del emisor es obligatorio")
            .Length(11)
            .WithMessage("El RUC debe tener exactamente 11 dígitos")
            .Matches(@"^\d{11}$")
            .WithMessage("El RUC debe contener solo dígitos numéricos");

        RuleFor(x => x.RazonSocialEmisor)
            .NotEmpty()
            .WithMessage("La razón social del emisor es obligatoria")
            .MaximumLength(500)
            .WithMessage("La razón social no puede exceder 500 caracteres");

        // RucReceptor es obligatorio solo para Facturas
        When(x => x.Tipo == "Factura", () =>
        {
            RuleFor(x => x.RucReceptor)
                .NotEmpty()
                .WithMessage("El RUC del receptor es obligatorio para Facturas")
                .Length(11)
                .WithMessage("El RUC debe tener exactamente 11 dígitos")
                .Matches(@"^\d{11}$")
                .WithMessage("El RUC debe contener solo dígitos numéricos");

            RuleFor(x => x.RazonSocialReceptor)
                .NotEmpty()
                .WithMessage("La razón social del receptor es obligatoria para Facturas")
                .MaximumLength(500)
                .WithMessage("La razón social no puede exceder 500 caracteres");
        });

        // Para Boletas, si se proporciona RUC, debe ser válido
        When(x => x.Tipo == "Boleta" && !string.IsNullOrEmpty(x.RucReceptor), () =>
        {
            RuleFor(x => x.RucReceptor)
                .Length(11)
                .WithMessage("El RUC debe tener exactamente 11 dígitos")
                .Matches(@"^\d{11}$")
                .WithMessage("El RUC debe contener solo dígitos numéricos");
        });

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Debe incluir al menos un item")
            .Must(items => items != null && items.Count > 0)
            .WithMessage("Debe incluir al menos un item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Descripcion)
                .NotEmpty()
                .WithMessage("La descripción del item es obligatoria")
                .MaximumLength(500)
                .WithMessage("La descripción no puede exceder 500 caracteres");

            item.RuleFor(i => i.Cantidad)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a 0");

            item.RuleFor(i => i.PrecioUnitario)
                .GreaterThan(0)
                .WithMessage("El precio unitario debe ser mayor a 0");
        });
    }

    private bool ValidarFormatoSerie(string tipo, string serie)
    {
        if (string.IsNullOrEmpty(tipo) || string.IsNullOrEmpty(serie))
            return false;

        if (tipo == "Factura")
            return serie.StartsWith("F") && serie.Length == 4 && serie.Substring(1).All(char.IsDigit);

        if (tipo == "Boleta")
            return serie.StartsWith("B") && serie.Length == 4 && serie.Substring(1).All(char.IsDigit);

        return false;
    }
}