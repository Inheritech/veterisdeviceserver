using Microsoft.EntityFrameworkCore.Migrations;

namespace VeterisDeviceServer.Migrations
{
    public partial class UserAccesses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServerUserAccesses",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    server_identifier = table.Column<string>(nullable: true),
                    user_identifier = table.Column<string>(nullable: true),
                    role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerUserAccesses", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerUserAccesses");
        }
    }
}
