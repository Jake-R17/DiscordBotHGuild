using DiscordBotGuild;
using DiscordBotHGuild.DBContext;
using DiscordBotHGuild.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.admin
{
    public class WarnC : BaseCommandModule
    {
        [Command("warn")]
        [RequirePermissions(Permissions.ManageMessages & Permissions.KickMembers)]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Hidden]
        public async Task Warn(CommandContext ctx, DiscordMember member = null, [RemainingText] string reason = "No reason provided")
        {
            if (ctx.Guild == null) { return; }
            if (member == null)
            {
                var noMember = new DiscordEmbedBuilder()
                    .WithTitle("Incorrect usage")
                    .WithDescription("Usage: .warn <member> <optional reason>")
                    .WithColor(new DiscordColor(255, 0, 0));

                await ctx.RespondAsync(embed: noMember).ConfigureAwait(false);
                return;
            }

            await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);

            var cannotWarn = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            // Explain why x can't be done
            string explanation;

            if (reason.Length > 50)
            {
                explanation = $"{Bot.nerdCross} The reason cannot exceed 50 characters!";
                await ctx.RespondAsync(embed: cannotWarn.WithDescription(explanation)).ConfigureAwait(false);
                return;
            }

            // Get the bot itself
            var bot = ctx.Guild.Members.FirstOrDefault(x => x.Value.Username == ctx.Client.CurrentUser.Username).Value;

            // Hierarchy checks. Top role (POS) to int.
            var botHierarchy = bot.Hierarchy;
            var memberHierarchy = member.Hierarchy;
            var summonerHierarchy = ctx.Member.Hierarchy;

            // Hierarchy checking
            if (member == ctx.Member)
            {
                explanation = $"{Bot.nerdCross} Cannot warn yourself!";
                await ctx.RespondAsync(embed: cannotWarn.WithDescription(explanation)).ConfigureAwait(false);
                return;
            }

            if (memberHierarchy < summonerHierarchy)
            {
                var warnEmbed = new DiscordEmbedBuilder()
                    .WithTitle($"{ctx.Member.Username} warned {member.Username}!")
                    .WithColor(new DiscordColor(255, 0, 0));

                await ctx.RespondAsync(embed: warnEmbed).ConfigureAwait(false);
                //await member.SendMessageAsync(embed: warnEmbed.AddField("Reason:", reason.ToString())).ConfigureAwait(false);

                var warn = new Warning
                {
                    GuildId = ctx.Guild.Id.ToString(),
                    MemberId = member.Id.ToString(),
                    WarnReason = reason.ToString(),
                    WarnDate = DateTime.UtcNow
                };

                using SqliteContext lite = new SqliteContext();
                lite.Warning.Add(warn);
                await lite.SaveChangesAsync().ConfigureAwait(false);
            }
            else if (memberHierarchy > botHierarchy || member.IsBot)
            {
                explanation = $"{Bot.nerdCross} I can't warn ``{member.DisplayName}#{member.Discriminator}``.";
                await ctx.RespondAsync(embed: cannotWarn.WithDescription(explanation)).ConfigureAwait(false);
            }
            else if (memberHierarchy >= summonerHierarchy)
            {
                explanation = $"{Bot.nerdCross} The specified user has equal or more permissions than you.";
                await ctx.RespondAsync(embed: cannotWarn.WithDescription(explanation)).ConfigureAwait(false);
            }
        }
    }
}
