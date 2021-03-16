using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.help
{
    [Group("ahelp")]
    [Hidden]
    public class HelpC : BaseCommandModule
    {
        [GroupCommand]
        public async Task Helplist(CommandContext ctx)
        {
            await ctx.Guild.GetAllMembersAsync().ConfigureAwait(false);
        }
    }
}
