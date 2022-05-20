using Microsoft.EntityFrameworkCore.Migrations;

namespace Sample.Api.Migrations
{
    public partial class paymentcardnumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FauldReason",
                table: "OrderState",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentCartNumber",
                table: "OrderState",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FauldReason",
                table: "OrderState");

            migrationBuilder.DropColumn(
                name: "PaymentCartNumber",
                table: "OrderState");
        }
    }
}
