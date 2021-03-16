using DiscordBotGuild;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.general
{
    public class ProfileC : BaseCommandModule
    {
        [Command("profile")]
        [Aliases("uinfo", "whois", "memberinfo", "account")]
        [Description("Shows some general information about a given member")]
        public async Task Profile(CommandContext ctx, DiscordMember member = null)
        {
            if (ctx.Guild == null) { return; }

            await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);

            member = (DiscordMember)(member ?? ctx.Member);

            // Get status
            var status = member.Presence.Status.ToString();
            var displayStatus = string.Empty;

            if (status == "Online") { displayStatus = $"{Bot.statOnline} Online"; }
            if (status == "Idle") { displayStatus = $"{Bot.statIdle} Idle"; }
            if (status == "DoNotDisturb") { displayStatus = $"{Bot.statDND} Do Not Disturb"; }
            if (status == "Offline") { displayStatus = $"{Bot.statOffline} Offline"; }
            if (status == "Streaming") { displayStatus = $"{Bot.statStreaming} Streaming"; }

            // Get activity
            var activity = member.Presence.Activity.ActivityType.ToString();

            if (activity == null) { activity = "none"; }
            if (activity == "ListeningTo") { activity = ":notes: Listening To Music"; }
            if (activity == "Playing") { activity = ":video_game: Playing a game"; }
            if (activity == "Streaming") { activity = ":red_circle: Streaming something fun"; }
            if (activity == "Custom") { activity = ":magic_wand: A custom status, interesting"; }
            if (activity == "Competing") { activity = ":crossed_swords: Competing in a game"; }

            // Get roles
            var roles = member.Roles.ToString();

            var profileEmbed = new DiscordEmbedBuilder()
                .WithTitle($"Info about {member.DisplayName}#{member.Discriminator}")

                .AddField("Username and UID", $"{member.DisplayName}#{member.Discriminator} | {member.Id}", true)
                .AddField("Status:", displayStatus, true)
                .AddField("Activity:", activity, true)

                .AddField("Account Created At:", member.CreationTimestamp.UtcDateTime.ToString(), true)
                .AddField("Joined Server At:", $"{member.JoinedAt.UtcDateTime:dd/M/yyyy}", true)
                .AddField("Roles:", roles, true)

                .WithThumbnail(member.AvatarUrl);

            await ctx.RespondAsync(embed: profileEmbed).ConfigureAwait(false);
        }
    }
}
