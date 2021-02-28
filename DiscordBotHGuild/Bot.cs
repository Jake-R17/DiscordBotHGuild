﻿using DiscordBotHGuild.commands;
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

            Commands.RegisterCommands<GeneralCommands>();
            Commands.RegisterCommands<ModerationCommands>();
            Commands.RegisterCommands<RoleCommands>();
            Commands.RegisterCommands<MoveMembers>();
            Commands.RegisterCommands<WeirdShit>();
            Commands.RegisterCommands<OwnerCommands>();

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

            // Discord emojis
            var botGuild = sender.Guilds.FirstOrDefault(x => x.Value.Id == 622059558340395008).Value;

            nerdCheckmark = botGuild.Emojis.FirstOrDefault(x => x.Value.Name == "nerd_checkmark").Value;
            nerdCross = botGuild.Emojis.FirstOrDefault(x => x.Value.Name == "nerd_cross").Value;
        }

        private async Task OnCommandFail(CommandsNextExtension ext, CommandErrorEventArgs e)
        {
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
                            await e.Context.RespondAsync($"{e.Context.User.Mention} you have insufficient permissions...");
                            return;
                        }
                    }
                }
            }
        }
    }
}
