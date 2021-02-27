using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands
{
    public class WeirdShit : BaseCommandModule
    {
        [Command("beloved")]
        [Description("Sophie's special command, BACK THE FUCK OFF!")]
        public async Task Beloved(CommandContext ctx)
        {
            if (ctx.Guild == null)
            {
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var FUCKYEAH = new DiscordEmbedBuilder()
                .WithTitle("Sophie, my beloved")
                .WithImageUrl("https://media1.tenor.com/images/3aabb2c2e0aecd36198e5646472b3dd6/tenor.gif?itemid=19641964");

            await ctx.RespondAsync(embed: FUCKYEAH).ConfigureAwait(false);
        }
    }
}
