using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Framework
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection WireUpDiscordFramework(this IServiceCollection services, string cmdPrefix)
        {
            services.AddSingleton<DiscordBotFramework>(s =>
            {
                var host = s.GetRequiredService<IHost>();
                var config = s.GetRequiredService<IConfiguration>();
                var logger = s.GetRequiredService<ILogger<DiscordBotFramework>>();
                var bot = new DiscordBotFramework(host, config, logger, cmdPrefix);
                return bot;
            });

            services.AddSingleton<CommandService>(s =>
            {
                var bot = s.GetRequiredService<DiscordBotFramework>();
                return bot.Commands;
            })
            .AddSingleton<GlobalPreferences>(s =>
            {
                var bot = s.GetRequiredService<DiscordBotFramework>();
                return bot.Preferences;
            });


            return services;
        }

    }
}
