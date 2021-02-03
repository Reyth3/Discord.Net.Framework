# Discord.Net.Framework

This is a framework for easily creating Discord bots with minimal amount of effort. All boilerplate code already in place plus some more advanced features allow for quick implementation of needed features instead of focusing on setting up the basics.

# Getting Started

To set the framework up, simply wire it up with dependency injection like so:

    var host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            services.WireUpDiscordFramework("a!"); // command prefix
        })
        .UseSerilog()
        .Build();

Then in `appsettings.json` add your Discord bot token and inject `DiscordBotFramework` into your service.
