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
    public class MuteC : BaseCommandModule
    {
        [Command("mute")]
        [Description("Mutes the specified user for the given amount of time")]
        [RequirePermissions(Permissions.MuteMembers)]
        [RequireBotPermissions(Permissions.ManageRoles)]
        [Hidden]
        public async Task Mute(CommandContext ctx, [Description("The member to mute")]DiscordMember member = null, 
            [Description("The reason (optional); why is x getting muted")]string reason = null)
        {
            if (ctx.Guild == null) { return; }
            if (member == null) 
            {
                var noMember = new DiscordEmbedBuilder()
                    .WithTitle("Incorrect usage")
                    .WithDescription("Usage: .mute <member> <reason>(optional)")
                    .WithColor(new DiscordColor(255, 0, 0));

                await ctx.RespondAsync(noMember).ConfigureAwait(false); 
                return; 
            }

            // Get muted role. If there is none, create role
            var mutedRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name.ToLower().Contains("muted")).Value;

            if (mutedRole == null)
            {
                await ctx.Guild.CreateRoleAsync("muted").ConfigureAwait(false);
                await ctx.RespondAsync("I just now created the muted role, be sure to put it in the correct position!").ConfigureAwait(false);
                mutedRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name.ToLower().Contains("muted")).Value;
            }
            if (member.Roles.Contains(mutedRole))
            {
                await ctx.RespondAsync("That user is already muted.").ConfigureAwait(false);
                return;
            }

            // Set permissions for the muted role
            mutedRole.Permissions.Revoke(Permissions.SendMessages);
            mutedRole.Permissions.Revoke(Permissions.Speak);
            mutedRole.Permissions.Revoke(Permissions.Stream);

            // Gets the bot within the guild
            var bot = ctx.Guild.Members.FirstOrDefault(x => x.Value.Username == ctx.Client.CurrentUser.Username).Value;

            // Hierarchy checks. Top role (POS) to int.
            var botHierarchy = bot.Hierarchy;
            var memberHierarchy = member.Hierarchy;
            var summonerHierarchy = ctx.Member.Hierarchy;

            // Hierarchy checking and executions
            if (memberHierarchy < botHierarchy)
            {
                await member.GrantRoleAsync(mutedRole).ConfigureAwait(false);
                await ctx.Message.CreateReactionAsync(Bot.nerdCheckmark).ConfigureAwait(false);

                if (reason != null)
                {
                    var muteEmbedDM = new DiscordEmbedBuilder()
                        .WithTitle($"You've been muted in ``{ctx.Guild.Name}`` by ``{ctx.Member.Username}#{ctx.Member.Discriminator}``")
                        .AddField("Reason:", reason.ToString())
                        .WithColor(new DiscordColor(255, 0, 0))
                        .WithThumbnail(ctx.Guild.IconUrl)
                        .WithFooter($" •  {bot.Username}  •  Date: {DateTime.UtcNow:dd/M/yyyy}", bot.AvatarUrl);

                    await member.SendMessageAsync(embed: muteEmbedDM).ConfigureAwait(false);
                }
            }
            else if (member == ctx.Member)
            {
                await ctx.RespondAsync($"{Bot.nerdCross} Cannot mute yourself!").ConfigureAwait(false);
            }
            else if (memberHierarchy >= summonerHierarchy)
            {
                await ctx.RespondAsync($"{Bot.nerdCross} The specified user has equal or more permissions than you.").ConfigureAwait(false);
            }
            else if (member.IsBot)
            {
                await ctx.RespondAsync($"{Bot.nerdCross} I cannot mute bots.").ConfigureAwait(false);
            }
        }
    }
}
