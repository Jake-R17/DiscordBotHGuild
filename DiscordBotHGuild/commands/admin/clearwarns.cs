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
    public class ClearwarnsC : BaseCommandModule
    {
        [Command("clear-warns")]
        [RequirePermissions(Permissions.ManageMessages & Permissions.KickMembers)]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Hidden]
        public async Task CW(CommandContext ctx, DiscordMember member = null)
        {
            if (ctx.Guild == null) { return; }
            if (member == null)
            {
                var noMember = new DiscordEmbedBuilder()
                    .WithTitle("Incorrect usage")
                    .WithDescription("Usage: .clear-warns <member>")
                    .WithColor(new DiscordColor(255, 0, 0));

                await ctx.RespondAsync(embed: noMember).ConfigureAwait(false);
                return;
            }

            var publicEmbed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            // Explain why x can't be done
            string explanation;

            // Hierarchies
            var memberHierarchy = member.Hierarchy;
            var summonerHierarchy = ctx.Member.Hierarchy;

            // Hierarchie and warn checking
            if (memberHierarchy <= summonerHierarchy)
            {
                using SqliteContext lite = new SqliteContext();
                var warnings = lite.Warning;

                var guildId = warnings.Any(x => x.GuildId == ctx.Guild.Id.ToString());
                var memberId = warnings.Any(x => x.MemberId == member.Id.ToString());

                int warnAmount = 0;

                foreach (var warn in warnings)
                {
                    if (guildId == true && memberId == true)
                    {
                        warnAmount++;
                    }
                }

                if (warnAmount == 0)
                {
                    explanation = $"{Bot.nerdCross} The specified member doesn't have any warnings.";
                    await ctx.RespondAsync(embed: publicEmbed.WithDescription(explanation)).ConfigureAwait(false);
                }
                else
                {
                    explanation = $"{Bot.nerdCheckmark} Removed all warnings regarding {member.Username}'s account!";
                    await ctx.RespondAsync(embed: publicEmbed.WithDescription(explanation).WithColor(new DiscordColor(0, 255, 0))).ConfigureAwait(false);
                }

                foreach (var warn in warnings)
                {
                    if (guildId == true && memberId == true)
                    {
                        lite.Warning.Remove(warn);
                        await lite.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
            else
            {
                explanation = $"{Bot.nerdCross} You can't remove warnings from people with higher permissions than you!";
                await ctx.RespondAsync(embed: publicEmbed.WithDescription(explanation)).ConfigureAwait(false);
            }
        }
    }
}
