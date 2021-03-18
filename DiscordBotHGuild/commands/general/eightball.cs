using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.general
{
    public class EightballC : BaseCommandModule
    {
        [Command("8ball")]
        [Aliases("8b")]
        [Cooldown(1, 1, CooldownBucketType.User)]
        public async Task Eightball(CommandContext ctx, [Description("Whatever you want an answer to, put it here :magic_wand:")][RemainingText] string question = null)
        {
            if (ctx.Guild == null) { return; }
            if (question == null) { await ctx.RespondAsync(":anger: Ask a question dummy!").ConfigureAwait(false); return; }

            await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);

            var replies = new List<string>
            {
                ":crystal_ball: **Yes**",
                ":crystal_ball: **Most definitely so!**",
                ":crystal_ball: **No**",
                ":crystal_ball: **Absolutely not!**",
                ":crystal_ball: **Maybe**",
                ":crystal_ball: **That's a question for later..**",
                ":crystal_ball: **I'm unsure about that...**",
                ":crystal_ball: **Perhaps..**",
                ":crystal_ball: **Possibly**",
                ":crystal_ball: **I don't think so**",
                ":crystal_ball: **That's a hard maybe!**",
                ":crystal_ball: **What is wrong with you?!**"
            };

            var answer = replies[new Random().Next(replies.Count)];

            await ctx.RespondAsync(answer).ConfigureAwait(false);
        }
    }
}
