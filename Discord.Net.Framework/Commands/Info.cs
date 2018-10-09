using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord.Net.Framework.Attributes;

namespace Discord.Net.Framework.Commands
{
    [HelpCategory("info")]
    class Info : ModuleBase<ExtendedCommandContext>
    {
        [Command("help"), Alias("h"), Summary("I will tell you what I can do."), Usage("a.help [command] - display a list of commands or detailed help for a specific command")]
        public async Task Help(string command = "")
        {
            var sb = new StringBuilder();
            var commands = Context.FrameworkInstance.Commands.Commands;
            if (string.IsNullOrEmpty(command))
            {
                var groupped = commands.GroupBy(o => o.Module);
                for (int i = 0; i < groupped.Count(); i++)
                {
                    sb.AppendLine($"{(groupped.ElementAt(i).Key.Attributes.FirstOrDefault(o => o is HelpCategoryAttribute) as HelpCategoryAttribute)?.Category ?? groupped.ElementAt(i).Key.Name.ToLower()}:");
                    foreach (var cmd in groupped.ElementAt(i))
                    {
                        if (cmd.Attributes.OfType<HideFromHelpAttribute>().FirstOrDefault() != null)
                            continue;
                        var name = cmd.Name;
                        var summary = cmd.Summary;
                        sb.AppendLine($"\t{name} - {summary};");
                    }
                }
                await ReplyAsync($"***List of Commands:*** ```{sb.ToString()}```\r\n```{Context.FrameworkInstance.CommandPrefix}help [command] - show help for a specific command```");
            }
            else
            {
                command = command.ToLower();
                var cmd = commands.FirstOrDefault(o => o.Name.ToLower() == command);
                if(cmd == null)
                {
                    await ReplyAsync("***Error:*** That command doesn't exist!");
                    return;
                }
                sb.AppendLine($"***Help for: {command}:***");
                var aliases = cmd.Aliases;
                var summary = cmd.Summary;
                var usage = cmd.Attributes.OfType<UsageAttribute>().FirstOrDefault()?.Text;
                if (usage == null)
                    usage = $"{Context.GuildSpecificPreferences.CommandPrefix}{cmd.Name} {string.Join(" ", cmd.Parameters.Select(o => o.IsOptional ? $"[{o.Name}='{o.DefaultValue}']" : o.Name))}";
                var category = cmd.Module.Attributes.OfType<HelpCategoryAttribute>().FirstOrDefault()?.Category;
                if (category == null) category = cmd.Module.Name.ToLower();
                sb.AppendLine($"**Aliases:** {(aliases != null ? string.Join(",", aliases.Select(o => $"`{o}`")) : "N/A")}");
                sb.AppendLine($"**Summary:** {summary}");
                sb.AppendLine($"**Usage:** ```{usage}```");
                sb.AppendLine($"**Command Category:** {category}");
                await ReplyAsync(sb.ToString());
            }
        }
    }
}
