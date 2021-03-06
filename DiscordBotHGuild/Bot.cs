using DiscordBotHGuild.commands;
using DiscordBotHGuild.commands.admin;
using DiscordBotHGuild.commands.admin.PRIVATE;
using DiscordBotHGuild.commands.general;
using DiscordBotHGuild.commands.help;
using DiscordBotHGuild.commands.test;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotGuild
{
    public partial class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        // Emojis
        public static DiscordEmoji nerdCheckmark;
        public static DiscordEmoji nerdCross;
        public static DiscordEmoji statOnline;
        public static DiscordEmoji statIdle;
        public static DiscordEmoji statDND;
        public static DiscordEmoji statOffline;
        public static DiscordEmoji statStreaming;

        public async Task RunAsync()
        {
            // JSON properties
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.All
            };

            // The client itself
            Client = new DiscordClient(config);
			
            // Client configs
			Client.Ready += OnClientReady;
            Client.GuildAvailable += OnGuildAvailable;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(5)
            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] {configJson.Prefix},
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = false,
            };

            // Command registration
            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.CommandErrored += OnCommandFail;

            // Registration of all commands (C = Command)
            Commands.RegisterCommands<BanC>();
            Commands.RegisterCommands<KickC>();
            Commands.RegisterCommands<MuteC>();
            Commands.RegisterCommands<PurgeC>();
            Commands.RegisterCommands<UnbanC>();
            Commands.RegisterCommands<UnmuteC>();
            Commands.RegisterCommands<VerificationC>();
            Commands.RegisterCommands<AvatarC>();
            Commands.RegisterCommands<EightballC>();
            Commands.RegisterCommands<MembersC>();
            Commands.RegisterCommands<ProfileC>();
            Commands.RegisterCommands<HelpC>();

            // PRIVATE
            Commands.RegisterCommands<MigrationC>();
            Commands.RegisterCommands<RebootC>();

            // EXTRA
            Commands.RegisterCommands<SophieC>();

            // TESTING
            Commands.RegisterCommands<TestC>();


            // Run bot, time infinite
            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        // Event arguments
        private Task OnClientReady(object sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

		private async Task OnGuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
        {
            // Discord status
            var status = new DiscordActivity(type: ActivityType.Playing, 
                name: "with my big tits");
            await Client.UpdateStatusAsync(status).ConfigureAwait(false);

            // Get the owner guild (In my case, "Step Off Nerd")
            var botGuild = sender.Guilds.FirstOrDefault(x => x.Value.Id == 622059558340395008).Value;

            // Default (yes/no)
            nerdCheckmark = botGuild.Emojis.FirstOrDefault(x => x.Value.Name == "ncheckmark").Value;
            nerdCross = botGuild.Emojis.FirstOrDefault(x => x.Value.Name == "ncross").Value;

            // Discord status
            statOnline = botGuild.Emojis.FirstOrDefault(x => x.Value.Name == "status_online").Value;
            statIdle = botGuild.Emojis.FirstOrDefault(x => x.Value.Name == "status_idle").Value;
            statDND = botGuild.Emojis.FirstOrDefault(x => x.Value.Name == "status_dnd").Value;
            statOffline = botGuild.Emojis.FirstOrDefault(x => x.Value.Name == "status_offline").Value;
            statStreaming = botGuild.Emojis.FirstOrDefault(x => x.Value.Name == "status_streaming").Value;
        }

        private async Task OnCommandFail(CommandsNextExtension ext, CommandErrorEventArgs e)
        {
            if (e.Exception is ArgumentException exc)
            {
                if (exc.Message == "Could not find a suitable overload for the command.")
                {
                    await e.Context.RespondAsync($"{Bot.nerdCross} that is not a valid user.");
                    return;
                }
            }

            var guild = ext.Client.Guilds.FirstOrDefault(x => x.Value.Id == 622059558340395008).Value;

            var debug = guild.Channels.FirstOrDefault(x => x.Value.Id == 812292213182431253).Value;

            if (debug != null)
            {
                if (e.Command == null)
                {
                    await debug.SendMessageAsync($"Command: {e.Context.Message} - Error: {e.Exception.Message} - Command caller: {e.Context.Member.DisplayName}").ConfigureAwait(false);
                }
                else
                {
                    await debug.SendMessageAsync($"Command: {e.Command.Name} - Error: {e.Exception.Message} - Command caller: {e.Context.Member.DisplayName}").ConfigureAwait(false);
                }
            }

            if (e.Exception is ChecksFailedException ex)
            {
                if (ex.FailedChecks.Count > 0)
                {
                    foreach (var f in ex.FailedChecks)
                    {
                        if (f.TypeId.ToString() == "DSharpPlus.CommandsNext.Attributes.CooldownAttribute")
                        {
                            await e.Context.RespondAsync($"{e.Context.User.Mention} please wait a bit before using this command again.");
                            return;
                        }
                        if (f.TypeId.ToString() == "DSharpPlus.CommandsNext.Attributes.RequirePermissionsAttribute")
                        {
                            await e.Context.RespondAsync($"{e.Context.User.Mention} you have insufficient permissions.");
                            return;
                        }
                        if (f.TypeId.ToString() == "DSharpPlus.CommandsNext.Attributes.RequireBotPermissionsAttribute")
                        {
                            await e.Context.RespondAsync($"{e.Context.User.Mention} I'm lacking certain permissions associated with this command");
                            return;
                        }
                    }
                }
            }
        }
    }
}
