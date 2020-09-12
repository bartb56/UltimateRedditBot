using Microsoft.EntityFrameworkCore.Migrations;

namespace UltimateRedditBot.Database.Migrations
{
    public partial class SettingDoneProperly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxBulkAdds",
                table: "GuildSettings");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "GuildSettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "GuildSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "GuildSettings");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "GuildSettings");

            migrationBuilder.AddColumn<int>(
                name: "MaxBulkAdds",
                table: "GuildSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
