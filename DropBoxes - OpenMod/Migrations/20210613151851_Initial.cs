using Microsoft.EntityFrameworkCore.Migrations;

namespace DropBoxes.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DropBoxes_LootBoxes",
                columns: table => new
                {
                    SteamId = table.Column<ulong>(nullable: false),
                    LootBoxId = table.Column<string>(nullable: false),
                    Amount = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropBoxes_LootBoxes", x => new { x.SteamId, x.LootBoxId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DropBoxes_LootBoxes");
        }
    }
}
