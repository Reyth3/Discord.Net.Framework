using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Framework
{
    public class ExtendedCommandContext : CommandContext
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


        public string FormatError(string message)
        {
            return $"***Error!*** *{message}*";
        }

        public string FormatInfo(string message)
        {
            return $"***{message}***";
        }

        public async Task<UserSearchResult> FindUser(string match)
        {
            if (match != "")
            {
                var users = (await Channel.GetUsersAsync(CacheMode.AllowDownload).Flatten());
                var search = users.Where(o => o.Username.ToLower().Contains(match.ToLower()) || Message.MentionedUserIds.Contains(o.Id) || o.ToString().ToLower().Contains(match.ToLower())).ToArray();
                if (search.Length == 0)
                {
                    return new UserSearchResult(UserSearchResult.UserSearchResultStatus.NoMatch);
                }
                else if (search.Length > 1)
                {
                    return new UserSearchResult(UserSearchResult.UserSearchResultStatus.MultipleMatches);
                }
                return new UserSearchResult(UserSearchResult.UserSearchResultStatus.FoundMatch, search.First());
            }
            else return new UserSearchResult(UserSearchResult.UserSearchResultStatus.FoundMatch, User);
        }
    }
}
