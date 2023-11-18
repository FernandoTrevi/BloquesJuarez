using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloquesJuarez.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRemitoyRemitoDetalleABaseDatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Remito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NroRemito = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    LugarEntrega = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Remito_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RemitoDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cantidad = table.Column<double>(type: "float", nullable: false),
                    PrecioUnitario = table.Column<double>(type: "float", nullable: false),
                    Subtotal = table.Column<double>(type: "float", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    RemitoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemitoDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemitoDetalle_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RemitoDetalle_Remito_RemitoId",
                        column: x => x.RemitoId,
                        principalTable: "Remito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Remito_ClienteId",
                table: "Remito",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RemitoDetalle_ProductoId",
                table: "RemitoDetalle",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_RemitoDetalle_RemitoId",
                table: "RemitoDetalle",
                column: "RemitoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemitoDetalle");

            migrationBuilder.DropTable(
                name: "Remito");
        }
    }
}
