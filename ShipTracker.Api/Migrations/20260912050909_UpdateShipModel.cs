using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShipModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "heading",
                table: "ships",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "speed",
                table: "ships",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "heading",
                table: "ships");

            migrationBuilder.DropColumn(
                name: "speed",
                table: "ships");
        }
    }
}
