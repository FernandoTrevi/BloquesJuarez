using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloquesJuarez.Migrations
{
    /// <inheritdoc />
    public partial class cambiosEnRemito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioUnitario",
                table: "RemitoDetalle");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "RemitoDetalle");

            migrationBuilder.AlterColumn<float>(
                name: "Cantidad",
                table: "RemitoDetalle",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "RemitoDetalle",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "RemitoDetalle");

            migrationBuilder.AlterColumn<double>(
                name: "Cantidad",
                table: "RemitoDetalle",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<double>(
                name: "PrecioUnitario",
                table: "RemitoDetalle",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Subtotal",
                table: "RemitoDetalle",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
