using DiscordBotGuild;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.general
{
    public class QuickpollTest : BaseCommandModule
    {
        [Command("quickpoll")]
        [Aliases("qp")]
        [Cooldown(1, 300, CooldownBucketType.User)]
        public async Task Quickpoll(CommandContext ctx, TimeSpan? span = null, [RemainingText] string input = null)
        {
            if (ctx.Guild == null) { return; }

            var publicEmbed = new DiscordEmbedBuilder()
                .WithTitle("Incorrect Usage")
                .WithColor(new DiscordColor(255, 0, 0));

            string d;

            if (span == null)
            {
                d = "Usage: .qp <timespan> <options>(At least 2, max 5)";
                await ctx.RespondAsync(embed: publicEmbed.WithDescription(d)).ConfigureAwait(false);
            }

            var split = input.Split(' ');

            List<string> options = new List<string>();

            foreach (string inputList in split)
            {
                options.Add(inputList);
            }

            if (options.Count == 1)
            {
                d = $"{Bot.nerdCross} Give at least 2 options!";
                await ctx.RespondAsync(embed: publicEmbed.WithDescription(d)).ConfigureAwait(false);
                return;
            }
            else if (options.Count == 2)
            {

            }
            else if (options.Count == 3)
            {

            }
            else if (options.Count == 4)
            {

            }
            else if (options.Count == 5)
            {

            }
        }
    }
}
