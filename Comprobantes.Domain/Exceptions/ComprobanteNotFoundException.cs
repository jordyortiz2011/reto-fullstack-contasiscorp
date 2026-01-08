namespace Comprobantes.Domain.Exceptions;

public class ComprobanteNotFoundException : DomainException
{
    public ComprobanteNotFoundException(Guid id) 
        : base($"Comprobante con ID {id} no fue encontrado")
    {
    }
}