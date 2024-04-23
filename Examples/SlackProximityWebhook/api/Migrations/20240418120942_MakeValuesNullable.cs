using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exercise5.Migrations
{
    /// <inheritdoc />
    public partial class MakeValuesNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOpen",
                table: "SensorValues",
                newName: "Proximity");

            migrationBuilder.AlterColumn<float>(
                name: "Temperature",
                table: "SensorValues",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "REAL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Proximity",
                table: "SensorValues",
                newName: "IsOpen");

            migrationBuilder.AlterColumn<float>(
                name: "Temperature",
                table: "SensorValues",
                type: "REAL",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "REAL",
                oldNullable: true);
        }
    }
}
