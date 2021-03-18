using DiscordBotGuild;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.admin
{
    public class UnbanC : BaseCommandModule
    {
        [Command("unban")]
        [Description("Unbans the specified user")]
        [RequirePermissions(Permissions.BanMembers)]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Hidden]
        public async Task Unban(CommandContext ctx, DiscordUser user = null)
        {
            if (ctx.Guild == null) { return; }
            if (user == null)
            {
                var noUser = new DiscordEmbedBuilder()
                    .WithTitle("Incorrect usage")
                    .WithDescription("Usage: .unban <member (ID)>")
                    .WithColor(new DiscordColor(255, 0, 0));

                await ctx.RespondAsync(embed: noUser).ConfigureAwait(false);
                return;
            }

            var unbanEmbed = new DiscordEmbedBuilder()
                .WithDescription($"{Bot.nerdCheckmark} Successfully unbanned {user.Mention}!")
                .WithColor(new DiscordColor(0, 255, 0));

            var notBannedEmbed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0))
                .WithDescription($"{Bot.nerdCross} That user isn't banned.");

            // Try to find the ban, on fail; catch, not banned
            try
            {
                await ctx.Guild.UnbanMemberAsync(user).ConfigureAwait(false);
                await ctx.RespondAsync(embed: unbanEmbed).ConfigureAwait(false);
            }
            catch
            {
                await ctx.RespondAsync(embed: notBannedEmbed).ConfigureAwait(false);
            }
        }
    }
}
