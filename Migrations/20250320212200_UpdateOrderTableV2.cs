using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderTableV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "total",
                table: "Order",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "taxAmount",
                table: "Order",
                newName: "TaxAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Total",
                table: "Order",
                newName: "total");

            migrationBuilder.RenameColumn(
                name: "TaxAmount",
                table: "Order",
                newName: "taxAmount");
        }
    }
}
