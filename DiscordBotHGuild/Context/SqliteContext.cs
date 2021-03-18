using DiscordBotHGuild.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotHGuild.DBContext
{
    public class SqliteContext : DbContext
    {
        public DbSet<MutedUser> MutedUsers { get; set; }
        public DbSet<Warning> Warning { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=Sqlite.db");
    }
}
