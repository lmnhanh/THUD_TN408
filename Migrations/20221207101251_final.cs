using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace THUD_TN408.Migrations
{
    public partial class final : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "Histories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 12, 7, 17, 12, 51, 389, DateTimeKind.Local).AddTicks(6989),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 12, 6, 14, 46, 31, 782, DateTimeKind.Local).AddTicks(6329));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "Histories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 12, 6, 14, 46, 31, 782, DateTimeKind.Local).AddTicks(6329),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 12, 7, 17, 12, 51, 389, DateTimeKind.Local).AddTicks(6989));
        }
    }
}
