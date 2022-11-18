using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace THUD_TN408.Migrations
{
    public partial class init1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateApply",
                schema: "dbo",
                table: "Prices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 17, 18, 14, 57, 958, DateTimeKind.Utc).AddTicks(2781),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 17, 18, 8, 59, 605, DateTimeKind.Utc).AddTicks(2517));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                schema: "dbo",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 17, 18, 14, 57, 958, DateTimeKind.Utc).AddTicks(995),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 17, 18, 8, 59, 605, DateTimeKind.Utc).AddTicks(690));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateApply",
                schema: "dbo",
                table: "Prices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 17, 18, 8, 59, 605, DateTimeKind.Utc).AddTicks(2517),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 17, 18, 14, 57, 958, DateTimeKind.Utc).AddTicks(2781));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                schema: "dbo",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 11, 17, 18, 8, 59, 605, DateTimeKind.Utc).AddTicks(690),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 11, 17, 18, 14, 57, 958, DateTimeKind.Utc).AddTicks(995));
        }
    }
}
