using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.general
{
    public class MembersC : BaseCommandModule
    {
        [Command("members")]
        [Aliases("m", "users")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        public async Task Members(CommandContext ctx)
        {
            if (ctx.Guild == null) { return; }

            await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);

            var total = await ctx.Guild.GetAllMembersAsync().ConfigureAwait(false);
            var countTotal = total.Count();
            var countBots = total.Count(x => x.IsBot);
            var guildMembers = countTotal - countBots;

            var totalsEmbed = new DiscordEmbedBuilder()
                .AddField("Members:", $"`{guildMembers}`", true)
                .AddField("Bots:", $"`{countBots}`", true)
                .AddField("Total:", $"`{countTotal}`")
                .WithColor(new DiscordColor(255, 105, 97))
                .WithThumbnail(ctx.Guild.IconUrl);

            await ctx.RespondAsync(embed: totalsEmbed).ConfigureAwait(false);
        }
    }
}
