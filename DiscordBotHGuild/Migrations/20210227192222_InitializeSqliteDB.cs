using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DiscordBotHGuild.Migrations
{
    public partial class InitializeSqliteDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MutedUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberId = table.Column<string>(type: "TEXT", nullable: true),
                    MutedReason = table.Column<string>(type: "TEXT", nullable: true),
                    MutedExpiration = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MutedUsers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MutedUsers");
        }
    }
}
