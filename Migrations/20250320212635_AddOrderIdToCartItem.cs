using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIdToCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Order_OrderId",
                table: "CartItem");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_OrderId",
                table: "CartItem");

            migrationBuilder.UpdateData(
                table: "CartItem",
                keyColumn: "OrderId",
                keyValue: null,
                column: "OrderId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "CartItem",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "CartItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_OrderId1",
                table: "CartItem",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Order_OrderId1",
                table: "CartItem",
                column: "OrderId1",
                principalTable: "Order",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Order_OrderId1",
                table: "CartItem");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_OrderId1",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "CartItem");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "CartItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_OrderId",
                table: "CartItem",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Order_OrderId",
                table: "CartItem",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");
        }
    }
}
