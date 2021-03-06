using DiscordBotGuild;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.admin
{
    public class PurgeC : BaseCommandModule
    {
        [Command("purge")]
        [Description("Removes the specified amount of messages within the specified channel")]
        [Cooldown(1, 5, CooldownBucketType.Channel)]
        [RequirePermissions(Permissions.ManageMessages)]
        [Hidden]
        public async Task Purge(CommandContext ctx, string input = null)
        {
            if (ctx.Guild == null) { return; }

            // Convert string to int. On fail; null
            Int32.TryParse(input, out int amount);

            if (amount > 1000)
            {
                amount = 1000;
            }

            // Message variables
            var messages = await ctx.Channel.GetMessagesAsync(amount).ConfigureAwait(false);
            var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);
            var count = filteredMessages.Count();

            // Embeds
            var purgeEmbed = new DiscordEmbedBuilder()
                .WithDescription($"Successfully deleted `{count}` message(s)")
                .WithColor(new DiscordColor(0, 255, 0))
                .WithFooter($" •  {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl);

            var errorEmbed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            // Error message, declared at if statement...
            string error;

            if (amount < 1 || String.IsNullOrWhiteSpace(input))
            {
                errorEmbed.WithTitle("Incorrect usage").WithDescription("Usage: .purge <amount>");
                await ctx.RespondAsync(embed: errorEmbed).ConfigureAwait(false);
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            // If statements and executions
            if (amount >= 1 && count >= 1)
            {
                await ctx.Channel.DeleteMessagesAsync(filteredMessages).ConfigureAwait(false);
                var purgeMessage = await ctx.RespondAsync(embed: purgeEmbed).ConfigureAwait(false);
                await Task.Delay(10000);
                await purgeMessage.DeleteAsync().ConfigureAwait(false);
            }
            else if (count == 0)
            {
                error = $"{Bot.nerdCross} Either the messages are too old or there are none to delete.";
                await ctx.RespondAsync(embed: errorEmbed.WithDescription(error)).ConfigureAwait(false);
            }
        }
    }
}
