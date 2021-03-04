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
        [Hidden]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Unban(CommandContext ctx, DiscordUser user = null)
        {
            if (ctx.Guild == null) { return; }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var notBannedEmbed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            // Info on why x failed and or cannot be done
            string info;

            if (user == null)
            {
                info = $"{Bot.nerdCross} Please specify a user.";
                await ctx.RespondAsync(embed: notBannedEmbed.WithDescription(info)).ConfigureAwait(false);
                return;
            }

            var unbanEmbed = new DiscordEmbedBuilder()
                .WithDescription($"{Bot.nerdCheckmark} Successfully unbanned {user.Mention}!")
                .WithColor(new DiscordColor(0, 255, 0));

            // Try to find the ban, on fail; catch, not banned
            try
            {
                await ctx.Guild.UnbanMemberAsync(user).ConfigureAwait(false);
                await ctx.RespondAsync(embed: unbanEmbed).ConfigureAwait(false);
            }
            catch
            {
                info = $"{Bot.nerdCross} That user isn't banned.";
                await ctx.RespondAsync(embed: notBannedEmbed.WithDescription(info)).ConfigureAwait(false);
            }
        }
    }
}
