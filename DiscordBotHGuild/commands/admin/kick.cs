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
    public class KickC : BaseCommandModule
    {
        [Command("kick")]
        [Description("Kicks the specified user from the guild")]
        [RequirePermissions(Permissions.KickMembers)]
        [Hidden]
        public async Task Kick(CommandContext ctx, DiscordMember member, [RemainingText] string reason = "Reason was not specified")
        {
            if (ctx.Guild == null) { return; }
            if (member == null)
            {
                var noUser = new DiscordEmbedBuilder()
                    .WithTitle("Incorrect usage")
                    .WithDescription("Usage: .kick <member> <reason>(optional)")
                    .WithColor(new DiscordColor(255, 0, 0));

                await ctx.RespondAsync(embed: noUser).ConfigureAwait(false);
                return;
            }

            // Gets the bot within the guild
            var bot = ctx.Guild.Members.FirstOrDefault(x => x.Value.Username == ctx.Client.CurrentUser.Username).Value;

            // Hierarchy checks. Top role (POS) to int.
            var botHierarchy = bot.Hierarchy;
            var memberHierarchy = member.Hierarchy;
            var summonerHierarchy = ctx.Member.Hierarchy;

            var kickEmbed = new DiscordEmbedBuilder()
                .WithDescription($"**{ctx.User.Mention} has kicked {member.Mention}**")
                .AddField("Reason:", reason.ToString())
                .WithColor(new DiscordColor(255, 0, 0));

            var kickEmbedDM = new DiscordEmbedBuilder()
                .WithTitle($"You were kicked from ``{ctx.Guild.Name}`` by ``{ctx.Member.Username}#{ctx.Member.Discriminator}``")
                .AddField("Reason:", reason.ToString())
                .WithColor(new DiscordColor(255, 0, 0))
                .WithThumbnail(ctx.Guild.IconUrl)
                .WithFooter($" •  {bot.Username}  •  Date: {DateTime.UtcNow:dd/M/yyyy}", bot.AvatarUrl);

            var cannotKick = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            // Explain why x can't be done
            string explanation;

            // Hierarchy checking and executions
            if (memberHierarchy < botHierarchy)
            {
                await member.SendMessageAsync(embed: kickEmbedDM).ConfigureAwait(false);
                await member.RemoveAsync().ConfigureAwait(false);
                await ctx.RespondAsync(embed: kickEmbed).ConfigureAwait(false);
            }
            else if (member == ctx.Member)
            {
                await ctx.RespondAsync($"{Bot.nerdCross} Cannot kick yourself!").ConfigureAwait(false);
            }
            else if (memberHierarchy > botHierarchy || member.IsBot)
            {
                explanation = $"{Bot.nerdCross} I cannot kick that user.";
                await ctx.RespondAsync(embed: cannotKick.WithDescription(explanation.ToString())).ConfigureAwait(false);
            }
            else if (memberHierarchy >= summonerHierarchy)
            {
                explanation = $"{Bot.nerdCross} The user you've specified has equal or more permissions than you!";
                await ctx.RespondAsync(embed: cannotKick.WithDescription(explanation.ToString())).ConfigureAwait(false);
            }
        }
    }
}
