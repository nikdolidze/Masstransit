using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sample.Api.Migrations
{
    public partial class saga2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "OrderState",
                newName: "Updated");

            migrationBuilder.AddColumn<string>(
                name: "CustomerNumber",
                table: "OrderState",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmitDate",
                table: "OrderState",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerNumber",
                table: "OrderState");

            migrationBuilder.DropColumn(
                name: "SubmitDate",
                table: "OrderState");

            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "OrderState",
                newName: "OrderDate");
        }
    }
}
