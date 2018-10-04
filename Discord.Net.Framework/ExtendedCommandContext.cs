using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.Net.Framework
{
    class ExtendedCommandContext : CommandContext
    {
        public ExtendedCommandContext(IDiscordClient client, IUserMessage message, DiscordBotFramework framework) :base(client, message)
        {
            FrameworkInstance = framework;
            if (Guild != null)
            {
                GuildSpecificPreferences = framework.Preferences.ServerSpecific.GetFor(Guild.Id);
                IsGuild = true;
            }
        }

        public bool IsGuild { get; set; }
        public DiscordBotFramework FrameworkInstance { get; set; }
        public SettingsDictionary GuildSpecificPreferences { get; set; }

    }
}
