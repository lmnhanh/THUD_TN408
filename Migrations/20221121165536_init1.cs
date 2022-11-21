using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace THUD_TN408.Migrations
{
    public partial class init1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "Products",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateApply",
                schema: "dbo",
                table: "Prices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 21, 16, 55, 36, 568, DateTimeKind.Utc).AddTicks(9598),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 21, 16, 50, 21, 938, DateTimeKind.Utc).AddTicks(8274));

            migrationBuilder.AlterColumn<bool>(
                name: "IsTrans",
                schema: "dbo",
                table: "Orders",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuccess",
                schema: "dbo",
                table: "Orders",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPaid",
                schema: "dbo",
                table: "Orders",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                schema: "dbo",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 21, 16, 55, 36, 568, DateTimeKind.Utc).AddTicks(7929),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 21, 16, 50, 21, 938, DateTimeKind.Utc).AddTicks(6177));

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "Categories",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateApply",
                schema: "dbo",
                table: "Prices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 21, 16, 50, 21, 938, DateTimeKind.Utc).AddTicks(8274),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 21, 16, 55, 36, 568, DateTimeKind.Utc).AddTicks(9598));

            migrationBuilder.AlterColumn<bool>(
                name: "IsTrans",
                schema: "dbo",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSuccess",
                schema: "dbo",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPaid",
                schema: "dbo",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                schema: "dbo",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 21, 16, 50, 21, 938, DateTimeKind.Utc).AddTicks(6177),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 21, 16, 55, 36, 568, DateTimeKind.Utc).AddTicks(7929));

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
