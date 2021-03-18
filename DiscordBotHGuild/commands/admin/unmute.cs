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
    public class UnmuteC : BaseCommandModule
    {
        [Command("unmute")]
        [RequirePermissions(Permissions.MuteMembers)]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Hidden]
        public async Task Unmute(CommandContext ctx, DiscordMember member = null)
        {
            if (ctx.Guild == null) { return; }

            var publicEmbed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            // Explain why x can't be done
            string explanation;

            if (member == null)
            {
                var noMember = new DiscordEmbedBuilder()
                    .WithTitle("Incorrect usage")
                    .WithDescription("Usage: .unmute <member>")
                    .WithColor(new DiscordColor(255, 0, 0));

                await ctx.RespondAsync(noMember).ConfigureAwait(false);
                return;
            }

            var mutedRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name.ToLower().Contains("muted")).Value;
            
            if (member.Roles.Contains(mutedRole))
            {
                await ctx.Message.CreateReactionAsync(Bot.nerdCheckmark).ConfigureAwait(false);
                await member.RevokeRoleAsync(mutedRole).ConfigureAwait(false);
                await member.SetMuteAsync(false).ConfigureAwait(false);
            }
            else
            {
                explanation = $"{Bot.nerdCross} The specified user isn't muted.";
                await ctx.RespondAsync(embed: publicEmbed.WithDescription(explanation)).ConfigureAwait(false);
            }
        }
    }
}
