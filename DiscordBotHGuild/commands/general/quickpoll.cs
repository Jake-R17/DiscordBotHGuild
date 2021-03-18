using DiscordBotGuild;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.general
{
    public class QuickpollC : BaseCommandModule
    {
        [Command("quickpoll")]
        [Aliases("qp")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task Quickpoll(CommandContext ctx, TimeSpan span, [RemainingText] string input = null)
        {
            if (ctx.Guild == null) { return; }

            var usageEmbed = new DiscordEmbedBuilder()
                .WithTitle("Incorrect Usage")
                .WithDescription("Usage: .quickpoll <question>")
                .WithColor(new DiscordColor(255, 0, 0));

            if (string.IsNullOrWhiteSpace(input) || span == null)
            {
                await ctx.RespondAsync(embed: usageEmbed).ConfigureAwait(false);
                return;
            }
            
            if (span.TotalMinutes > 5)
            {
                await ctx.RespondAsync("Max time is 5 min, I've set it to that.").ConfigureAwait(false);
            }

            var qpEmbed = new DiscordEmbedBuilder()
                .WithTitle(input)
                .WithColor(new DiscordColor(255, 215, 0));

            var message = await ctx.RespondAsync(embed: qpEmbed).ConfigureAwait(false);

            // Get time
            double inputTime = span.TotalSeconds;

            for (double time = inputTime; time > 0; time--)
            {
                if (!message.ToString().Contains("Time Left:"))
                {
                    await message.ModifyAsync((DiscordEmbed)qpEmbed.AddField("Time Left:", time.ToString())).ConfigureAwait(false);
                }

                Console.WriteLine(time);
                await ctx.Channel.SendMessageAsync($"{time}").ConfigureAwait(false);
                await Task.Delay(1000);
            }
            return;
        }
    }
}
