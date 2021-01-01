using Discord.Commands;
using Discord.Net.Framework.Enums;
using Discord.Net.WebSockets;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Net.Framework
{
    public class DiscordBotFramework
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly IHost _host;
        public DiscordBotFramework(IHost host, IConfiguration config, ILogger<DiscordBotFramework> logger, string cmdPrefix)
        {
            _host = host;
            _config = config;
            _logger = logger;
            CommandPrefix = cmdPrefix;
            Client = new DiscordSocketClient();
            Commands = new CommandService();
            R = new Random();
            Preferences = GlobalPreferences.LoadFromFile("");
            Preferences.SetValue("prefix", cmdPrefix);
            FollowUpContexts = new Dictionary<ulong, CommandFollowUpContext>();
            Client.Ready += async () =>
            {
                _logger.LogInformation("Discord bot is ready.");
                _logger.LogInformation("Logged in as: {0}", Client.CurrentUser);
            };
            Client.MessageReceived += Client_MessageReceived;

        }

        public DiscordSocketClient Client { get; set; }
        internal CommandService Commands { get; set; }
        public Random R { get; set; }
        public GlobalPreferences Preferences { get; set; }
        public string CommandPrefix { get; set; }
        public string InstanceId { get; set; }
        internal Dictionary<ulong, CommandFollowUpContext> FollowUpContexts { get; set; }

        private string forcedToken;

        public void ForceToken(string token)
        {
            forcedToken = token;
        }

        public async Task RunAsync()
        {
            var token = _config["Discord:Token"];
            if (token == null)
            {
                throw new ArgumentNullException("The token has not been set correctly.");
            }
            else
            {
                await Client.LoginAsync(TokenType.Bot, token);
                await Client.StartAsync();
            }

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
            {
                var followUp = FollowUpContexts[message.Author.Id];
                followUp.Context = new ExtendedCommandContext(Client, message, this);
                OnFollowUpMessageReceived(followUp);
            }
            else if (message.HasStringPrefix(prefix, ref argPos))
            {
                var context = new ExtendedCommandContext(Client, message, this);
                var result = await Commands.ExecuteAsync(context, argPos, _host.Services);
                if (!result.IsSuccess)
                {
                    _logger.LogError("Commands", $"{(context.Guild != null ? context.Guild.Name : "DMs")}  > {message.Author}: {message.Content} ({(result.IsSuccess ? "Success" : result.Error.ToString())})", result.IsSuccess ? LogType.Success : LogType.Warning);
                    await message.Channel.SendMessageAsync($"**Error!** *{result.ErrorReason}*");
                }
                else _logger.LogInformation("Commands", $"{(context.Guild != null ? context.Guild.Name : "DMs")}  > {message.Author}: {message.Content} ({(result.IsSuccess ? "Success" : result.Error.ToString())})", result.IsSuccess ? LogType.Success : LogType.Warning);
            }
        }

        public async Task ImportCommandsAsync(Assembly assembly)
        {
            await Commands.AddModulesAsync(assembly, _host.Services);
        }

        public async Task ImportCommandsAsync(params Type[] modules)
        {
            foreach(var t in modules)
                await Commands.AddModuleAsync(t, _host.Services);
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
