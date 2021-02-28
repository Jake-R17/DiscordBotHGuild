using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBotHGuild.Migrations
{
    public partial class UpdateMutedDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuildId",
                table: "MutedUsers",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "MutedUsers");
        }
    }
}
