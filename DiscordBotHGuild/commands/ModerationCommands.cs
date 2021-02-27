using DiscordBotGuild;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands
{
    public class ModerationCommands : BaseCommandModule
    {
        // COMPLETE
        [Command("purge")]
        [Description("Removes the specified amount of messages within the specified channel")]
        [Cooldown(1, 5, CooldownBucketType.Channel)]
        [Hidden]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task Purge(CommandContext ctx, string input = null)
        {
            if (ctx.Guild == null)
            {
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            // Convert string to int. On fail; null
            Int32.TryParse(input, out int amount);

            if (amount > 1000)
            {
                amount = 1000;
            }

            // Message variables
            var messages = await ctx.Channel.GetMessagesAsync(amount).ConfigureAwait(false);

            var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

            var count = filteredMessages.Count();

            // Embeds
            string error;

            var purgeEmbed = new DiscordEmbedBuilder()
                .WithDescription($"Successfully deleted `{count}` message(s)")
                .WithColor(new DiscordColor(0, 255, 0))
                .WithFooter($" •  {ctx.User.Username}#{ctx.User.Discriminator}", ctx.User.AvatarUrl);

            var errorEmbed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            // If statements and executions
            if (amount >= 1 && count >= 1)
            {
                await ctx.Channel.DeleteMessagesAsync(filteredMessages).ConfigureAwait(false);
                var purgeMessage = await ctx.Channel.SendMessageAsync(embed: purgeEmbed).ConfigureAwait(false);
                await Task.Delay(10000);
                await purgeMessage.DeleteAsync().ConfigureAwait(false);
            }
            else if (amount < 1 || String.IsNullOrWhiteSpace(input))
            {
                error = $"{Bot.nerdCross} Please enter a valid number of messages you want to delete.";
                await ctx.Channel.SendMessageAsync(embed: errorEmbed.WithDescription(error)).ConfigureAwait(false);
            }
            else if (count == 0)
            {
                error = $"{Bot.nerdCross} Either the messages are too old or there are none to delete.";
                await ctx.Channel.SendMessageAsync(embed: errorEmbed.WithDescription(error)).ConfigureAwait(false);
            }
        }

        // COMPLETE
        [Command("ban")]
        [Description("Bans the specified user, requires the 'Ban Members' permission")]
        [Hidden]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, DiscordMember user = null, [RemainingText] string reason = "reason was not specified")
        {
            if (ctx.Guild == null)
            {
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            // Get the bot you're checking can ban
            var bot = ctx.Guild.Members.FirstOrDefault(x => x.Value.Username == ctx.Client.CurrentUser.Username.ToString()).Value;

            var noUser = new DiscordEmbedBuilder()
                .WithDescription($"{Bot.nerdCross} Please specify a user or their ID")
                .WithColor(new DiscordColor(255, 0, 0));

            if (user == null)
            {
                await ctx.Channel.SendMessageAsync(embed: noUser).ConfigureAwait(false);
                return;
            }

            var banEmbed = new DiscordEmbedBuilder()
                .WithDescription($"**{ctx.User.Mention} has banned {user.Mention}**")
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

            // Explain why x can't be done
            string explanation = String.Empty;

            var cannotBan = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            // Make lists
            List<DiscordRole> summonerRoles = new List<DiscordRole>();
            List<DiscordRole> serverRoles = new List<DiscordRole>();
            List<DiscordRole> botRoles = new List<DiscordRole>();
            List<DiscordRole> memberRoles = new List<DiscordRole>();

            // Add to lists
            foreach (var sur in ctx.Member.Roles.OrderByDescending(x => x.Position))
            {
                summonerRoles.Add(sur);
            }

            foreach (var sr in ctx.Guild.Roles.OrderByDescending(x => x.Value.Position))
            {
                serverRoles.Add(sr.Value);
            }

            foreach (var br in bot.Roles.OrderByDescending(x => x.Position))
            {
                botRoles.Add(br);
            }

            foreach (var mr in user.Roles.OrderByDescending(x => x.Position))
            {
                memberRoles.Add(mr);
            }

            // Get the first/top roles of x
            var summonerTop = summonerRoles[0];

            Console.WriteLine(summonerTop);

            var botTop = botRoles[0];

            Console.WriteLine(botTop);

            var memberTop = memberRoles[0];

            Console.WriteLine(memberTop);

            // If's and executions. Checking role logic
            if (serverRoles.IndexOf(botTop) < serverRoles.IndexOf(memberTop))
            {
                await user.SendMessageAsync(embed: banEmbedDM).ConfigureAwait(false);
                await user.BanAsync().ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(embed: banEmbed).ConfigureAwait(false);
            }
            else if (serverRoles.IndexOf(botTop) > serverRoles.IndexOf(memberTop) || user.IsBot)
            {
                explanation = $"{Bot.nerdCross} I can't ban that user.";
                await ctx.Channel.SendMessageAsync(embed: cannotBan.WithDescription(explanation)).ConfigureAwait(false);
            }
            else if (serverRoles.IndexOf(summonerTop) >= serverRoles.IndexOf(memberTop))
            {
                explanation = $"{Bot.nerdCross} The specified user has equal or more permissions than you.";
                await ctx.Channel.SendMessageAsync(embed: cannotBan.WithDescription(explanation)).ConfigureAwait(false);
            }
        }

        // COMPLETE
        [Command("unban")]
        [Description("Unbans the specified user")]
        [Hidden]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Unban(CommandContext ctx, DiscordUser user = null)
        {
            if (ctx.Guild == null)
            {
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var notBannedEmbed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0));

            // User is not (yet) specified
            string info;

            // If no user is given, return
            if (user == null)
            {
                info = $"{Bot.nerdCross} Please specify a user.";
                await ctx.Channel.SendMessageAsync(embed: notBannedEmbed.WithDescription(info.ToString()));
                return;
            }

            // User is specified
            var unbanEmbed = new DiscordEmbedBuilder()
                .WithDescription($"{Bot.nerdCheckmark} Successfully unbanned {user.Mention}!")
                .WithColor(new DiscordColor(0, 255, 0));

            // Try to find the ban, on fail, return no ban
            try
            {
                await ctx.Guild.UnbanMemberAsync(user).ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(embed: unbanEmbed).ConfigureAwait(false);
            }
            catch
            {
                info = $"{Bot.nerdCross} That user isn't banned.";
                await ctx.Channel.SendMessageAsync(embed: notBannedEmbed.WithDescription(info.ToString())).ConfigureAwait(false);
            }
        }
    }

    [Group("move")]
    [Hidden]
    public class MoveMembers : BaseCommandModule
    {
        // COMPLETE
        [GroupCommand]
        [Description("Moves all players or just the specified player from call x to x")]
        [RequirePermissions(Permissions.MoveMembers)]
        public async Task Move(CommandContext ctx, DiscordMember member = null, DiscordChannel toChannel = null)
        {
            if (ctx.Guild == null)
            {
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            if (member == null)
            {
                await ctx.RespondAsync("Specify a member").ConfigureAwait(false);
                return;
            }
            else if (toChannel == null)
            {
                await ctx.Channel.SendMessageAsync("Specify a channel").ConfigureAwait(false);
                return;
            }

            await member.PlaceInAsync(toChannel).ConfigureAwait(false);
            await ctx.RespondAsync($"Moved {member.Mention} to `{toChannel.Name}`");
        }

        // COMPLETE
        [Command("all")]
        [Description("Moves all players from call x to x")]
        [RequirePermissions(Permissions.MoveMembers)]
        public async Task Moveall(CommandContext ctx, string fromChannel = null, string toChannel = null)
        {
            if (ctx.Guild == null)
            {
                return;
            }

            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            if (fromChannel == null)
            {
                await ctx.RespondAsync("Specify the channel you want to move everyone out of.");
                return;
            }
            if (toChannel == null)
            {
                await ctx.RespondAsync("Specify the channel you want to move everyone to.");
                return;
            }

            var toChannelByString = ctx.Guild.Channels.FirstOrDefault(x => x.Value.Name.ToLower() == toChannel && x.Value.Type == ChannelType.Voice).Value;
            var toChannelById = ctx.Guild.Channels.FirstOrDefault(x => x.Value.Id.ToString() == toChannel && x.Value.Type == ChannelType.Voice).Value;
            var vcChannelByString = ctx.Guild.Channels.FirstOrDefault(x => x.Value.Name.ToLower() == fromChannel && x.Value.Type == ChannelType.Voice).Value;
            var vcChannelById = ctx.Guild.Channels.FirstOrDefault(x => x.Value.Id.ToString() == fromChannel && x.Value.Type == ChannelType.Voice).Value;

            if (toChannelByString == null || vcChannelByString == null)
            {
                if (toChannelById == null && vcChannelById == null)
                {
                    return;
                }

                foreach (var u in vcChannelById.Users)
                {
                    try
                    {
                        await u.PlaceInAsync(toChannelById).ConfigureAwait(false);
                    }
                    catch
                    {
                        await ctx.RespondAsync("failed to execute").ConfigureAwait(false);
                    }
                }
                return;
            }

            foreach (var u in vcChannelByString.Users)
            {
                try
                {
                    await u.PlaceInAsync(toChannelByString).ConfigureAwait(false);
                }
                catch
                {
                    await ctx.RespondAsync("failed to execute").ConfigureAwait(false);
                }
            }

        }
    }
}
