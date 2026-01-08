using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comprobantes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comprobantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Serie = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RucEmisor = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    RazonSocialEmisor = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RucReceptor = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    RazonSocialReceptor = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SubTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IGV = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comprobantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComprobanteItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ComprobanteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobanteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprobanteItems_Comprobantes_ComprobanteId",
                        column: x => x.ComprobanteId,
                        principalTable: "Comprobantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComprobanteItems_ComprobanteId",
                table: "ComprobanteItems",
                column: "ComprobanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_Estado",
                table: "Comprobantes",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_FechaEmision",
                table: "Comprobantes",
                column: "FechaEmision");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_RucReceptor",
                table: "Comprobantes",
                column: "RucReceptor");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_Serie_Numero",
                table: "Comprobantes",
                columns: new[] { "Serie", "Numero" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComprobanteItems");

            migrationBuilder.DropTable(
                name: "Comprobantes");
        }
    }
}
