using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.general
{
    public class AvatarC : BaseCommandModule
    {
        [Command("avatar")]
        [Aliases("av", "img", "image", "pfp")]
        [Description("Shows the profile picture of you or the given member")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        public async Task Avatar(CommandContext ctx, DiscordMember member = null)
        {
            if (ctx.Guild == null) { return; }

            member ??= ctx.Member;

            var avatarDisplay = new DiscordEmbedBuilder()
                .WithTitle($"Avatar of:")
                .WithDescription(member.Mention)
                .WithImageUrl(member.AvatarUrl)
                .WithFooter($" •  Requested by: {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl);

            await ctx.RespondAsync(embed: avatarDisplay).ConfigureAwait(false);
        }
    }
}
