using Microsoft.EntityFrameworkCore.Migrations;

namespace VeterisDeviceServer.Migrations
{
    public partial class Translations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "serialized_device_trans",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    serialized = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serialized_device_trans", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "serialized_device_trans");
        }
    }
}
