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
    public class WarningsC : BaseCommandModule
    {
        [Command("warnings")]
        [RequirePermissions(Permissions.ManageMessages & Permissions.KickMembers)]
        [Hidden]
        public async Task Warnings(CommandContext ctx, DiscordMember member = null)
        {
            if (ctx.Guild == null) { return; }
            if (member == null)
            {
                var noMember = new DiscordEmbedBuilder()
                    .WithTitle("Incorrect usage")
                    .WithDescription("Usage: .warnings <member>")
                    .WithColor(new DiscordColor(255, 0, 0));

                await ctx.RespondAsync(embed: noMember).ConfigureAwait(false);
                return;
            }

            await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);

            var publicEmbed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            var warningsEmbed = new DiscordEmbedBuilder()
                .WithTitle($"Warnings of {member.Username}")
                .WithThumbnail(member.AvatarUrl)
                .WithColor(new DiscordColor(153, 179, 255));

            // Explain why x can't be done
            string explanation;

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
                    warningsEmbed.AddField("Reason:", $"``{warn.WarnReason}``", true).AddField("Date (GMT):", $"``{warn.WarnDate.Date:d}``", true).AddField("ID:", $"``{warn.Id}``", true);
                }
            }

            if (warnAmount == 0)
            {
                explanation = $"{Bot.nerdCross} The specified member doesn't have any warnings";
                await ctx.RespondAsync(embed: publicEmbed.WithDescription(explanation)).ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync(embed: warningsEmbed).ConfigureAwait(false);
            }
        }
    }
}
