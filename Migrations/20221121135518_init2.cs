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
                defaultValue: new DateTime(2022, 11, 21, 13, 55, 17, 754, DateTimeKind.Utc).AddTicks(2590),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 20, 16, 18, 44, 2, DateTimeKind.Utc).AddTicks(7690));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                schema: "dbo",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 21, 13, 55, 17, 753, DateTimeKind.Utc).AddTicks(9378),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 20, 16, 18, 44, 2, DateTimeKind.Utc).AddTicks(5680));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateApply",
                schema: "dbo",
                table: "Prices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 20, 16, 18, 44, 2, DateTimeKind.Utc).AddTicks(7690),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 21, 13, 55, 17, 754, DateTimeKind.Utc).AddTicks(2590));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                schema: "dbo",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 20, 16, 18, 44, 2, DateTimeKind.Utc).AddTicks(5680),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 21, 13, 55, 17, 753, DateTimeKind.Utc).AddTicks(9378));
        }
    }
}
