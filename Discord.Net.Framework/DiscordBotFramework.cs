﻿using Discord.Commands;
using Discord.Net.Framework.Enums;
using Discord.Net.WebSockets;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Net.Framework
{
    public class DiscordBotFramework
    {
        private static Dictionary<string, DiscordBotFramework> _instances;

        public DiscordBotFramework(string cmdPrefix, string instanceId = "")
        {
            if (_instances == null)
                _instances = new Dictionary<string, DiscordBotFramework>();
            Client = new DiscordSocketClient();
            _instances.Add(instanceId, this);
            Commands = new CommandService();
            R = new Random();
            Preferences = GlobalPreferences.LoadFromFile(instanceId);
            Preferences.SetValue("prefix", cmdPrefix);
            CommandPrefix = cmdPrefix;
            InstanceId = instanceId;
            FollowUpContexts = new Dictionary<ulong, CommandFollowUpContext>();
        }

        public DiscordSocketClient Client { get; set; }
        public IServiceProvider Services { get; set; }
        internal CommandService Commands { get; set; }
        public Random R { get; set; }
        public GlobalPreferences Preferences { get; set; }
        public string CommandPrefix { get; set; }
        public string InstanceId { get; set; }
        internal Dictionary<ulong, CommandFollowUpContext> FollowUpContexts { get; set; }


        public void Log(string module, string message, LogType logType = LogType.Success)
        {
            try
            {
                var now = DateTime.Now;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[{0} {1}] ({2}) ", now.ToShortDateString(), now.ToShortTimeString(), module);
                Console.ForegroundColor = logType == LogType.Success ? ConsoleColor.Green : logType == LogType.Warning ? ConsoleColor.DarkYellow : ConsoleColor.Red;
                Console.Write("{0}", message);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("\r\n");
                var raw = $"{logType}: [{now.ToShortDateString()} {now.ToShortTimeString()}] ({module}) {message}";
                var dir = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "cmdlog"));
                using (var str = File.AppendText(Path.Combine(dir.FullName, $"log_{DateTime.Today:yyyyMMdd}.txt")))
                    str.WriteLine(raw);
            }
            catch (Exception ex)
            {
            }

        }

        public async Task RunAsync()
        {
            if (Services == null)
                throw new NullReferenceException("The services have to be configured before attempting to run the bot.");
            Client.Ready += async () =>
            {
                Log("Bot", "Bot ready");
                Log("Bot", $"Logged in as: {Client.CurrentUser}");
            };
            Client.MessageReceived += Client_MessageReceived;
            var tokenPath = Path.Combine(Directory.GetCurrentDirectory(), "token.txt");
            string token = "";
            do
            {
                if (!File.Exists(tokenPath))
                    File.WriteAllText(tokenPath, "");
                token = File.ReadAllText(tokenPath);
                if (token == "")
                {
                    throw new ArgumentNullException("The token is not specified in the 'token.txt' file.");
                }
                else
                {
                    await Client.LoginAsync(TokenType.Bot, token);
                    await Client.StartAsync();
                }
            } while (token == "");

            await Commands.AddModuleAsync<Commands.Info>(Services);
            await Commands.AddModuleAsync<Commands.Admin>(Services);
        }

        public ServiceProvider ConfigureServices(params Type[] services)
        {
            var builder = new ServiceCollection();
            builder.AddSingleton<DiscordSocketClient>(Client)
                .AddSingleton<CommandService>(Commands)
                .AddSingleton<GlobalPreferences>(Preferences)
                .AddSingleton<Random>(R);

            foreach (var service in services)
                builder.AddSingleton(service);

            var provider = builder.BuildServiceProvider();
            Services = provider;
            return provider;
        }

        private async Task Client_MessageReceived(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;
            var guild = (message.Channel as IGuildChannel)?.Guild;
            var guildId = guild != null ? guild.Id : 0;
            var prefix = Preferences.ServerSpecific.GetFor(guildId).CommandPrefix;
            if (FollowUpContexts.ContainsKey(message.Author.Id) && FollowUpContexts[message.Author.Id] != null)
                OnFollowUpMessageReceived(FollowUpContexts[message.Author.Id]);
            else if (message.HasStringPrefix(prefix, ref argPos))
            {
                var context = new ExtendedCommandContext(Client, message, this);
                var result = await Commands.ExecuteAsync(context, argPos, Services);
                if (!result.IsSuccess)
                    await message.Channel.SendMessageAsync($"**Error!** *{result.ErrorReason}*");
                Log("Commands", $"{context.Guild.Name} > {message.Author}: {message.Content} ({(result.IsSuccess ? "Success" : result.Error.ToString())})", result.IsSuccess ? LogType.Success : LogType.Warning);
            }
        }

        public async Task ImportCommandsAsync(Assembly assembly)
        {
            await Commands.AddModulesAsync(assembly, Services);
        }

        public async Task ImportCommandsAsync(params Type[] modules)
        {
            foreach(var t in modules)
                await Commands.AddModuleAsync(t, Services);
        }

        public static DiscordBotFramework GetInstance(string id)
        {
            if (_instances.ContainsKey(id))
                return _instances[id];
            else return null;
        }


        #region Events
        public event Action<object, CommandFollowUpContext> FollowUpMessageReceived;

        internal void OnFollowUpMessageReceived(CommandFollowUpContext context)
        {
            FollowUpMessageReceived?.Invoke(this, context);
        }

        #endregion
    }
}
