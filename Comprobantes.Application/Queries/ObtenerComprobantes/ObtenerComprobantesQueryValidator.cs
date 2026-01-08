using FluentValidation;

namespace Comprobantes.Application.Queries.ObtenerComprobantes;

public class ObtenerComprobantesQueryValidator : AbstractValidator<ObtenerComprobantesQuery>
{
    public ObtenerComprobantesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("El número de página debe ser mayor a 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("El tamaño de página debe ser mayor a 0")
            .LessThanOrEqualTo(50)
            .WithMessage("El tamaño de página no puede ser mayor a 50");

        RuleFor(x => x.FechaDesde)
            .LessThanOrEqualTo(x => x.FechaHasta)
            .When(x => x.FechaDesde.HasValue && x.FechaHasta.HasValue)
            .WithMessage("La fecha desde debe ser menor o igual a la fecha hasta");

        RuleFor(x => x.RucReceptor)
            .Length(11)
            .WithMessage("El RUC debe tener exactamente 11 dígitos")
            .Matches(@"^\d{11}$")
            .WithMessage("El RUC debe contener solo dígitos numéricos")
            .When(x => !string.IsNullOrEmpty(x.RucReceptor));
    }
}