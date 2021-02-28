using DiscordBotGuild;
using DiscordBotHGuild.DBContext;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands
{
    public class OwnerCommands : BaseCommandModule
    {
        [Command("reboot")]
        [Description("Reboots the bot. ONLY USE IF NECESSARY!")]
        [Hidden]
        public async Task Reboot(CommandContext ctx)
        {
            if (ctx.Guild == null)
            {
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            if (ctx.User.Id == 621045916517924866 || ctx.User.Id == 304278201553911808 || ctx.User.Id == 385038858691280896)
            {
                await ctx.RespondAsync($"{ctx.User.Mention} RESTARTING! PLEASE WAIT AT LEAST 3 SECONDS BEFORE USING COMMANDS!..");

                // Disconnect the client
                ctx.Client.Dispose();

                // Get new bot instance
                var bot = new Bot();

                // Restart the bot
                bot.RunAsync().GetAwaiter().GetResult();
            }
            else
            {
                return;
            }
        }

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
