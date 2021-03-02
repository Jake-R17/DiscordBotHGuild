using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.admin
{
    public class VerificationC : BaseCommandModule
    {
        [Command("verify")]
        [Aliases("continue")]
        public async Task Verify(CommandContext ctx)
        {
            if (ctx.Guild == null)
            {
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            // Check for role
            var role = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name.ToLower().Contains("verified")).Value;

            if (role == null)
            {
                await ctx.RespondAsync("There is no role called 'verified', please create this role to add a verification method.").ConfigureAwait(false);
                return;
            }

            // Embed
            var verifyEmbed = new DiscordEmbedBuilder()
                .WithTitle("React to the white checkmark to verify")
                .WithColor(new DiscordColor(50, 180, 130))
                .WithFooter($"  • {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl);

            // Send message, create variable
            var verifyMessage = await ctx.Channel.SendMessageAsync(embed: verifyEmbed.WithThumbnail(ctx.User.AvatarUrl)).ConfigureAwait(false);

            // Get reactions
            var verifyAgree = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
            var verifyDisagree = DiscordEmoji.FromName(ctx.Client, ":x:");

            // Add reactions
            await verifyMessage.CreateReactionAsync(verifyAgree).ConfigureAwait(false);
            await verifyMessage.CreateReactionAsync(verifyDisagree).ConfigureAwait(false);

            var interactivity = ctx.Client.GetInteractivity();

            // Delete message (30s)
            _ = Task.Run(() => DeleteMessage(verifyMessage));

            // Wait for reaction
            var reactionResult = await interactivity.WaitForReactionAsync(
                x => x.Message == verifyMessage &&
                x.User == ctx.User &&
                (x.Emoji == verifyAgree || x.Emoji == verifyDisagree)).ConfigureAwait(false);

            // Check results
            if (reactionResult.Result == null)
            {
            }
            else if (reactionResult.Result.Emoji == verifyAgree)
            {
                await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);
            }
            else if (reactionResult.Result.Emoji == verifyDisagree)
            {
                await ctx.Member.RevokeRoleAsync(role).ConfigureAwait(false);
            }

            await verifyMessage.DeleteAsync().ConfigureAwait(false);
        }

        public async void DeleteMessage(DiscordMessage msg)
        {
            await Task.Delay(30000);
            try
            {
                await msg.DeleteAsync().ConfigureAwait(false);
            }
            catch
            {
                return;
            }
        }
    }
}
