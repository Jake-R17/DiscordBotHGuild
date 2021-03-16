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
        [RequirePermissions(Permissions.MuteMembers)]
        [Hidden]
        public async Task Mute(CommandContext ctx, DiscordMember member = null, [RemainingText]string reason = null)
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

            // Public embe explanation
            string d = string.Empty;

            var publicEmbed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            if (member == ctx.Member)
            {
                d = $"{Bot.nerdCross} Cannot mute yourself!";
                await ctx.RespondAsync(embed: publicEmbed.WithDescription(d)).ConfigureAwait(false);
                return;
            }

            // Get mute role. If there is none, create one
            var mutedRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name.ToLower().Contains("muted")).Value;

            if (mutedRole == null)
            {
                await ctx.Guild.CreateRoleAsync("Muted", null, (new DiscordColor(94, 94, 94))).ConfigureAwait(false);
                mutedRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name.ToLower().Contains("muted")).Value;
            }

            // Set overrides unless they are already set (mutedRole)
            foreach (var ch in ctx.Guild.Channels)
            {
                var overwrites = ch.Value.PermissionOverwrites;

                if (ch.Value.Type == ChannelType.Text || ch.Value.Type == ChannelType.Category)
                {
                    if (overwrites.Any(x => x.Id == mutedRole.Id) == false || overwrites == null)
                    {
                        await ch.Value.AddOverwriteAsync(mutedRole, Permissions.None, Permissions.SendMessages).ConfigureAwait(false);
                    }
                }
            }

            if (member.Roles.Contains(mutedRole))
            {
                d = $"{Bot.nerdCross} The specified user is already muted.";
                await ctx.RespondAsync(embed: publicEmbed.WithDescription(d)).ConfigureAwait(false);
                return;
            }

            // Gets the bot within the guil
            var bot = ctx.Guild.Members.FirstOrDefault(x => x.Value.Username == ctx.Client.CurrentUser.Username).Value;

            // Hierarchy checks. Top role (POS) to int.
            var botHierarchy = bot.Hierarchy;
            var memberHierarchy = member.Hierarchy;
            var summonerHierarchy = ctx.Member.Hierarchy;

            // Hierarchy checking an executions
            if (memberHierarchy < summonerHierarchy)
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

                if (!member.IsMuted)
                {
                    await member.SetMuteAsync(true).ConfigureAwait(false);
                }
            }
            else if (memberHierarchy >= summonerHierarchy)
            {
                await ctx.RespondAsync($"{Bot.nerdCross} The specified user has equal or more permissions than you.").ConfigureAwait(false);
            }
            else if (memberHierarchy > botHierarchy || member.IsBot)
            {
                await ctx.RespondAsync($"{Bot.nerdCross} I cannot mute **{member.DisplayName}#{member.Discriminator}**").ConfigureAwait(false);
            }
        }
    }
}
