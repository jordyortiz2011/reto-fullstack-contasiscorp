using Comprobantes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Comprobantes.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Comprobante> Comprobantes { get; set; }
    public DbSet<ComprobanteItem> ComprobanteItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Comprobante
        modelBuilder.Entity<Comprobante>(entity =>
        {
            entity.ToTable("Comprobantes");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Tipo)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(c => c.Serie)
                .IsRequired()
                .HasMaxLength(4);

            entity.Property(c => c.Numero)
                .IsRequired();

            entity.Property(c => c.FechaEmision)
                .IsRequired();

            entity.Property(c => c.RucEmisor)
                .IsRequired()
                .HasMaxLength(11);

            entity.Property(c => c.RazonSocialEmisor)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(c => c.RucReceptor)
                .HasMaxLength(11);

            entity.Property(c => c.RazonSocialReceptor)
                .HasMaxLength(500);

            entity.Property(c => c.SubTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(c => c.IGV)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(c => c.Total)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(c => c.Estado)
                .HasConversion<string>()
                .IsRequired();

            // Relación con Items
            entity.HasMany(c => c.Items)
                .WithOne(i => i.Comprobante)
                .HasForeignKey(i => i.ComprobanteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para mejorar búsquedas
            entity.HasIndex(c => c.FechaEmision);
            entity.HasIndex(c => c.RucReceptor);
            entity.HasIndex(c => new { c.Serie, c.Numero }).IsUnique();
            entity.HasIndex(c => c.Estado);
        });

        // Configuración de ComprobanteItem
        modelBuilder.Entity<ComprobanteItem>(entity =>
        {
            entity.ToTable("ComprobanteItems");
            entity.HasKey(i => i.Id);

            entity.Property(i => i.Descripcion)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(i => i.Cantidad)
                .HasPrecision(18, 3)
                .IsRequired();

            entity.Property(i => i.PrecioUnitario)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(i => i.Subtotal)
                .HasPrecision(18, 2)
                .IsRequired();
        });
    }
}