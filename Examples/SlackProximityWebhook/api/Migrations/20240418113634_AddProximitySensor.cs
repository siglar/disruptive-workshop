using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exercise5.Migrations
{
    /// <inheritdoc />
    public partial class AddProximitySensor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsOpen",
                table: "SensorValues",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOpen",
                table: "SensorValues");
        }
    }
}
