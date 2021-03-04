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
    public class MuteC : BaseCommandModule
    {
        [Command("mute")]
        [Description("Mutes the specified player for the given amount of time")]
        [RequirePermissions(Permissions.MuteMembers)]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [Hidden]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        public async Task Mute(CommandContext ctx, [Description("The member to mute")]DiscordMember member = null,
            [Description("How long to mute (optional); 1h|1d|1w, etc")]TimeSpan? amount = null, [Description("The reason; why is x getting muted")]string reason = "none")
        {
            if (ctx.Guild == null) { return; }
            if (member == null) 
            { 
                await ctx.RespondAsync($"{Bot.nerdCross} Pleas specify a user or their ID").ConfigureAwait(false); 
                return; 
            }

            // Get muted role. If there is none, create role
            var mutedRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name.ToLower() == "muted").Value;
            if (mutedRole == null)
            {
                await ctx.Guild.CreateRoleAsync("muted").ConfigureAwait(false);
                mutedRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name.ToLower() == "muted").Value;
            }
            if (member.Roles.Contains(mutedRole)) { await ctx.RespondAsync("That user is already muted.").ConfigureAwait(false); return; }

            // Set permissions for the muted role
            await mutedRole.ModifyAsync("muted", mutedRole.Permissions.Revoke(Permissions.SendMessages)).ConfigureAwait(false);
            await mutedRole.ModifyAsync("muted", mutedRole.Permissions.Revoke(Permissions.Speak)).ConfigureAwait(false);
            await mutedRole.ModifyAsync("muted", mutedRole.Permissions.Revoke(Permissions.Stream)).ConfigureAwait(false);

            if (amount == null)
            {
                await member.GrantRoleAsync(mutedRole).ConfigureAwait(false);
                await ctx.Message.CreateReactionAsync(Bot.nerdCheckmark).ConfigureAwait(false);
                return;
            }

            // Still needs to be configured

            // Configures the mute to set in the database
            var newMute = new MutedUser
            {
                GuildId = $"{ctx.Guild.Id}",
                MemberId = $"{member.Id}",
                MutedReason = $"{reason}",
                MutedExpiration = DateTime.UtcNow.AddDays(3)
            };

            using (SqliteContext lite = new SqliteContext())
            {
                lite.MutedUsers.Add(newMute);
                await lite.SaveChangesAsync().ConfigureAwait(false);

                await ctx.RespondAsync($"Added a {member.Username}#{member.Discriminator} to the mutes").ConfigureAwait(false);
            }
        }
    }
}
