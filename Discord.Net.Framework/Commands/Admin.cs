using Discord.Commands;
using Discord.Net.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Framework.Commands
{
    [RequireUserPermission(GuildPermission.Administrator)]
    //[HelpCategory("admin")]
    public class Admin : ModuleBase<ExtendedCommandContext>
    {
        [Command("setprefix"), Alias("prefix"), Summary("Set a custom prefix for this server")]
        public async Task SetPrefix(string prefix = "")
        {
            if(prefix.Length > 4)
                await ReplyAsync(Context.FormatError("Specified prefix is too long."));
            else if(prefix == "")
            {
                Context.GuildSpecificPreferences.CommandPrefix = null;
                await ReplyAsync(Context.FormatInfo($"The prefix has been reset to the default value: {Context.FrameworkInstance.CommandPrefix}"));
            }
            else
            {
                Context.GuildSpecificPreferences.CommandPrefix = prefix;
                await ReplyAsync(Context.FormatInfo($"Command prefix set to `{prefix}`"));
            }
        }
    }
}
