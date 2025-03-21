using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderTableWithUserIdddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Address_ShippingAddressId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryMethods_DeliveryMethoddId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentSummary_PaymentSummaryyId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_DeliveryMethoddId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_PaymentSummaryyId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ShippingAddressId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeliveryMethoddId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentSummaryyId",
                table: "Order");

            migrationBuilder.AlterColumn<int>(
                name: "ShippingAddressId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryMethodId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentSummaryId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryMethodId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentSummaryId",
                table: "Order");

            migrationBuilder.AlterColumn<int>(
                name: "ShippingAddressId",
                table: "Order",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryMethoddId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentSummaryyId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeliveryMethoddId",
                table: "Order",
                column: "DeliveryMethoddId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PaymentSummaryyId",
                table: "Order",
                column: "PaymentSummaryyId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ShippingAddressId",
                table: "Order",
                column: "ShippingAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Address_ShippingAddressId",
                table: "Order",
                column: "ShippingAddressId",
                principalTable: "Address",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DeliveryMethods_DeliveryMethoddId",
                table: "Order",
                column: "DeliveryMethoddId",
                principalTable: "DeliveryMethods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PaymentSummary_PaymentSummaryyId",
                table: "Order",
                column: "PaymentSummaryyId",
                principalTable: "PaymentSummary",
                principalColumn: "Id");
        }
    }
}
