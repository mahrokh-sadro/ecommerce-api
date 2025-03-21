using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderTableWithUserIdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryMethods_DeliveryMethodId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentSummary_PaymentSummaryId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "PaymentSummaryId",
                table: "Order",
                newName: "PaymentSummaryyId");

            migrationBuilder.RenameColumn(
                name: "DeliveryMethodId",
                table: "Order",
                newName: "DeliveryMethoddId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_PaymentSummaryId",
                table: "Order",
                newName: "IX_Order_PaymentSummaryyId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_DeliveryMethodId",
                table: "Order",
                newName: "IX_Order_DeliveryMethoddId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryMethods_DeliveryMethoddId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentSummary_PaymentSummaryyId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "PaymentSummaryyId",
                table: "Order",
                newName: "PaymentSummaryId");

            migrationBuilder.RenameColumn(
                name: "DeliveryMethoddId",
                table: "Order",
                newName: "DeliveryMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_PaymentSummaryyId",
                table: "Order",
                newName: "IX_Order_PaymentSummaryId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_DeliveryMethoddId",
                table: "Order",
                newName: "IX_Order_DeliveryMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DeliveryMethods_DeliveryMethodId",
                table: "Order",
                column: "DeliveryMethodId",
                principalTable: "DeliveryMethods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PaymentSummary_PaymentSummaryId",
                table: "Order",
                column: "PaymentSummaryId",
                principalTable: "PaymentSummary",
                principalColumn: "Id");
        }
    }
}
