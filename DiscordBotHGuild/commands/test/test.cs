using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotHGuild.commands.test
{
    public class TestC : BaseCommandModule
    {
        [Group("move")]
        [Hidden]
        public class MoveCS : BaseCommandModule
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
                    await ctx.RespondAsync("Specify a channel").ConfigureAwait(false);
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
                    await ctx.RespondAsync("Specify the channel you want to move everyone out of.").ConfigureAwait(false);
                    return;
                }
                if (toChannel == null)
                {
                    await ctx.RespondAsync("Specify the channel you want to move everyone to.").ConfigureAwait(false);
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
}
