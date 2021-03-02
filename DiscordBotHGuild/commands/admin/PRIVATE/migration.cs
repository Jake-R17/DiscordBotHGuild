using DiscordBotHGuild.DBContext;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.admin.PRIVATE
{
    public class MigrationC : BaseCommandModule
    {
        [Command("migrate")]
        [Hidden]
        public async Task Migrate(CommandContext ctx)
        {
            if (ctx.User.Id == 621045916517924866)
            {
                Console.WriteLine("*** MIGRATING... ***");

                await using SqliteContext lite = new SqliteContext();

                if (lite.Database.GetPendingMigrationsAsync().Result.Any())
                {
                    await lite.Database.MigrateAsync();
                }

                await ctx.Channel.SendMessageAsync($"Sqlite Migration complete.");
            }
            else
            {
                return;
            }
        }
    }
}
