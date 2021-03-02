using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.general
{
    public class ProfileC : BaseCommandModule
    {
        [Command("profile")]
        [Aliases("p", "pf")]
        [Description("Shows some general information about a given user")]
        public async Task Profile(CommandContext ctx, DiscordMember user = null)
        {
            if (ctx.Guild == null)
            {
                return;
            }

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
