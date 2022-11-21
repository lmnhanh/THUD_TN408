using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace THUD_TN408.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateApply",
                schema: "dbo",
                table: "Prices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 21, 17, 27, 2, 898, DateTimeKind.Utc).AddTicks(1338),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 21, 16, 55, 36, 568, DateTimeKind.Utc).AddTicks(9598));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                schema: "dbo",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 21, 17, 27, 2, 898, DateTimeKind.Utc).AddTicks(65),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 21, 16, 55, 36, 568, DateTimeKind.Utc).AddTicks(7929));

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                schema: "dbo",
                table: "Carts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Carts",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCheckedOut",
                schema: "dbo",
                table: "Carts",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateApply",
                schema: "dbo",
                table: "Prices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 21, 16, 55, 36, 568, DateTimeKind.Utc).AddTicks(9598),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 21, 17, 27, 2, 898, DateTimeKind.Utc).AddTicks(1338));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                schema: "dbo",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 21, 16, 55, 36, 568, DateTimeKind.Utc).AddTicks(7929),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 21, 17, 27, 2, 898, DateTimeKind.Utc).AddTicks(65));

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                schema: "dbo",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCheckedOut",
                schema: "dbo",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
