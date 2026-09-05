using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ShipTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ships",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mmsi = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    speed_over_ground = table.Column<double>(type: "double precision", nullable: true),
                    course_over_ground = table.Column<double>(type: "double precision", nullable: true),
                    destination = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    last_updated_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ships", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ships_last_updated_utc",
                table: "ships",
                column: "last_updated_utc");

            migrationBuilder.CreateIndex(
                name: "ix_ships_mmsi",
                table: "ships",
                column: "mmsi",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ships");
        }
    }
}
