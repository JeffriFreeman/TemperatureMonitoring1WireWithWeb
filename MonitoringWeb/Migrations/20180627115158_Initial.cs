using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MonitoringWeb.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Number = table.Column<int>(nullable: false),
                    Id = table.Column<string>(nullable: false),
                    Tempreture = table.Column<double>(nullable: false),
                    Mount = table.Column<string>(nullable: true),
                    CRC = table.Column<bool>(nullable: false),
                    LastGet = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorsData",
                columns: table => new
                {
                    Number = table.Column<int>(nullable: false),
                    Id = table.Column<string>(nullable: false),
                    Tempreture = table.Column<double>(nullable: false),
                    Mount = table.Column<string>(nullable: true),
                    CRC = table.Column<bool>(nullable: false),
                    LastGet = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorsData", x => new { x.Id, x.LastGet });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "SensorsData");
        }
    }
}
