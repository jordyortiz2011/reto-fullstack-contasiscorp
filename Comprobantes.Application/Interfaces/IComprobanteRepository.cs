using Comprobantes.Domain.Entities;
using Comprobantes.Domain.Enums;

namespace Comprobantes.Application.Interfaces;

public interface IComprobanteRepository
{
    Task<Comprobante?> GetByIdAsync(Guid id);
    Task<(List<Comprobante> Items, int TotalCount)> GetAllAsync(
        int page,
        int pageSize,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        TipoComprobante? tipo = null,
        string? rucReceptor = null,
        EstadoComprobante? estado = null);
    Task<int> GetNextNumeroAsync(string serie);
    Task<Comprobante> AddAsync(Comprobante comprobante);
    Task UpdateAsync(Comprobante comprobante);
    Task<bool> ExistsAsync(Guid id);
}