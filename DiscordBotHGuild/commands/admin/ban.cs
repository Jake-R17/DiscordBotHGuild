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
    public class BanC : BaseCommandModule
    {
        [Command("ban")]
        [Description("Bans the specified user, requires the 'Ban Members' permission")]
        [RequirePermissions(Permissions.BanMembers)]
        [Hidden]
        public async Task Ban(CommandContext ctx, DiscordMember member = null, [RemainingText] string reason = "reason was not specified")
        {
            if (ctx.Guild == null)
            {
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var noUser = new DiscordEmbedBuilder()
                .WithDescription($"{Bot.nerdCross} Please specify a user or their ID")
                .WithColor(new DiscordColor(255, 0, 0));

            if (member == null)
            {
                await ctx.RespondAsync(embed: noUser).ConfigureAwait(false);
                return;
            }

            // Gets the bot within the guild
            var bot = ctx.Guild.Members.FirstOrDefault(x => x.Value.Username == ctx.Client.CurrentUser.Username.ToString()).Value;

            // Hierarchy checks. Top role (POS) to int.
            var botHierarchy = bot.Hierarchy;
            var memberHierarchy = member.Hierarchy;
            var summonerHierarchy = ctx.Member.Hierarchy;

            var banEmbed = new DiscordEmbedBuilder()
                .WithDescription($"**{ctx.User.Mention} has banned {member.Mention}**")
                .AddField("Reason:", reason.ToString())
                .WithColor(new DiscordColor(255, 0, 0))
                .WithThumbnail("https://media1.tenor.com/images/ef7a7efecb259c77e77720ce991b5c4a/tenor.gif?itemid=19698186")
                .WithFooter($" •  {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl);

            var banEmbedDM = new DiscordEmbedBuilder()
                .WithTitle($"You've been banned from ``{ctx.Guild.Name}`` by ``{ctx.Member.Username}#{ctx.Member.Discriminator}``")
                .AddField("Reason:", reason.ToString())
                .WithColor(new DiscordColor(255, 0, 0))
                .WithThumbnail(ctx.Guild.IconUrl)
                .WithFooter($" •  {bot.Username}  •  Date: {DateTime.UtcNow:dd/M/yyyy}", bot.AvatarUrl);

            var cannotBan = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            // Explain why x can't be done
            string explanation = String.Empty;

            // Hierarchy checking and executions
            if (memberHierarchy < botHierarchy)
            {
                await member.SendMessageAsync(embed: banEmbedDM).ConfigureAwait(false);
                await member.BanAsync().ConfigureAwait(false);
                await ctx.RespondAsync(embed: banEmbed).ConfigureAwait(false);
            }
            else if (memberHierarchy > botHierarchy || member.IsBot)
            {
                explanation = $"{Bot.nerdCross} I can't ban that user.";
                await ctx.RespondAsync(embed: cannotBan.WithDescription(explanation.ToString())).ConfigureAwait(false);
            }
            else if (memberHierarchy >= summonerHierarchy)
            {
                explanation = $"{Bot.nerdCross} The specified user has equal or more permissions than you.";
                await ctx.RespondAsync(embed: cannotBan.WithDescription(explanation.ToString())).ConfigureAwait(false);
            }
        }
    }
}
