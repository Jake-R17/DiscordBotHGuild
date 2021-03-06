using DiscordBotGuild;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.admin.PRIVATE
{
    public class RebootC : BaseCommandModule
    {
        [Command("reboot")]
        [Hidden]
        public async Task Reboot(CommandContext ctx)
        {
            if (ctx.User.Id == 621045916517924866 || ctx.User.Id == 304278201553911808 || ctx.User.Id == 385038858691280896)
            {
                await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);

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
    }
}
