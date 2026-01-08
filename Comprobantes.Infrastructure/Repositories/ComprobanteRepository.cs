using Comprobantes.Application.Interfaces;
using Comprobantes.Domain.Entities;
using Comprobantes.Domain.Enums;
using Comprobantes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Comprobantes.Infrastructure.Repositories;

public class ComprobanteRepository : IComprobanteRepository
{
    private readonly ApplicationDbContext _context;

    public ComprobanteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Comprobante?> GetByIdAsync(Guid id)
    {
        return await _context.Comprobantes
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(List<Comprobante> Items, int TotalCount)> GetAllAsync(
        int page,
        int pageSize,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        TipoComprobante? tipo = null,
        string? rucReceptor = null,
        EstadoComprobante? estado = null)
    {
        var query = _context.Comprobantes
            .Include(c => c.Items)
            .AsQueryable();

        // Aplicar filtros con fechas normalizadas a UTC
        if (fechaDesde.HasValue)
        {
            // Normalizar a UTC si no lo es
            var fechaDesdeUtc = fechaDesde.Value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(fechaDesde.Value, DateTimeKind.Utc)
                : fechaDesde.Value.ToUniversalTime();

            query = query.Where(c => c.FechaEmision >= fechaDesdeUtc);
        }

        if (fechaHasta.HasValue)
        {
            // Normalizar a UTC y agregar 23:59:59 para incluir todo el día
            var fechaHastaUtc = fechaHasta.Value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(fechaHasta.Value.Date.AddDays(1).AddSeconds(-1), DateTimeKind.Utc)
                : fechaHasta.Value.Date.AddDays(1).AddSeconds(-1).ToUniversalTime();

            query = query.Where(c => c.FechaEmision <= fechaHastaUtc);
        }

        if (tipo.HasValue)
        {
            query = query.Where(c => c.Tipo == tipo.Value);
        }

        if (!string.IsNullOrWhiteSpace(rucReceptor))
        {
            query = query.Where(c => c.RucReceptor == rucReceptor);
        }

        if (estado.HasValue)
        {
            query = query.Where(c => c.Estado == estado.Value);
        }

        // Contar total
        var totalCount = await query.CountAsync();

        // Aplicar paginación y ordenar por fecha descendente
        var items = await query
            .OrderByDescending(c => c.FechaEmision)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<int> GetNextNumeroAsync(string serie)
    {
        var ultimoNumero = await _context.Comprobantes
            .Where(c => c.Serie == serie)
            .MaxAsync(c => (int?)c.Numero);

        return (ultimoNumero ?? 0) + 1;
    }

    public async Task<Comprobante> AddAsync(Comprobante comprobante)
    {
        await _context.Comprobantes.AddAsync(comprobante);
        await _context.SaveChangesAsync();
        return comprobante;
    }

    public async Task UpdateAsync(Comprobante comprobante)
    {
        _context.Comprobantes.Update(comprobante);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Comprobantes.AnyAsync(c => c.Id == id);
    }
}