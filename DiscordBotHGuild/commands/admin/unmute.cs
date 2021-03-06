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
        [Hidden]
        public async Task Unmute(CommandContext ctx, DiscordMember member = null)
        {
            if (ctx.Guild == null) { return; }
            if (member == null)
            {
                await ctx.RespondAsync($"{Bot.nerdCross} Please specify a user to unmute.").ConfigureAwait(false);
                return;
            }

            var mutedRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name.ToLower().Contains("muted")).Value;
            
            if (member.Roles.Contains(mutedRole))
            {
                await member.RevokeRoleAsync(mutedRole).ConfigureAwait(false);
                await ctx.Message.CreateReactionAsync(Bot.nerdCheckmark).ConfigureAwait(false);
            }
            else
            {
                await ctx.RespondAsync($"{Bot.nerdCross} The specified user isn't muted.").ConfigureAwait(false);
            }
        }
    }
}
