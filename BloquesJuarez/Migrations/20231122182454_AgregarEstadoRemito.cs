using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloquesJuarez.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadoRemito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Remito",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Remito");
        }
    }
}
