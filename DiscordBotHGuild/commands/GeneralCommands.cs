using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands
{
    public class GeneralCommands : BaseCommandModule
    {
        // COMPLETE
        [Command("avatar")]
        [Aliases("av", "img", "image", "pfp")]
        [Description("Shows the profile picture of you or the given user")]
        public async Task Avatar(CommandContext ctx, DiscordMember user = null)
        {
            if (ctx.Guild == null)
            {
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            user = (DiscordMember)(user ?? ctx.Member);

            var avatarDisplay = new DiscordEmbedBuilder()
                .WithTitle($"Avatar of:")
                .WithDescription(user.Mention)
                .WithImageUrl(user.AvatarUrl)
                .WithFooter($" •  Requested by: {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl);

            await ctx.RespondAsync(embed: avatarDisplay).ConfigureAwait(false);
        }

        // BUSY / Unsure if I'm gonna keep this
        [Command("profile")]
        [Aliases("p","pf")]
        [Description("Shows some general information about a given user")]
        public async Task Profile(CommandContext ctx, DiscordMember user = null)
        {
            if (ctx.Guild == null)
            {
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            user = (DiscordMember)(user ?? ctx.Member);

            var profileEmbed = new DiscordEmbedBuilder()
                .WithTitle($"Profile of {user.DisplayName}#{user.Discriminator}")
                .AddField("Joined at:", $"`{user.CreationTimestamp:dd/M/yyyy}`", true)
                .AddField("Joined server at:", $"`{user.JoinedAt.UtcDateTime:dd/M/yyyy}`", true)
                .AddField("Presence type:", $"`{user.Presence.Status}`", true)
                .AddField("Activity type:", $"`{user.Presence.Activity.ActivityType}`", true)
                .WithThumbnail(user.AvatarUrl);

            await ctx.RespondAsync(embed: profileEmbed).ConfigureAwait(false);
        }
    }
}
