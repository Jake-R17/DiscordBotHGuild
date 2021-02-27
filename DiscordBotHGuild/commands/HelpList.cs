using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
namespace DiscordBotHGuild.commands
{
    [Group("help")]
    [Aliases("h")]
    public class HelpList : BaseCommandModule
    {
        [GroupCommand]
        [Description("Displays all public commands")]
        public async Task Helplist(CommandContext ctx)
        {
            await ctx.Guild.GetAllMembersAsync().ConfigureAwait(false);
        }
    }
}
