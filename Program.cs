using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;



    GatewayClient client;
    string token = Environment.GetEnvironmentVariable("BotToken") ?? "NzY1MjYwMDIxNDIzNjY5Mjk4.GOZrKs.0qDANqhdNNEGh3u-3aPMSwsZlorG30m6G071d0";
    string prefix = Environment.GetEnvironmentVariable("BotPrefix") ?? "#";
    client = new GatewayClient(new BotToken(token), new GatewayClientConfiguration
    {
        Logger = new ConsoleLogger(),
        Intents =  GatewayIntents.AllNonPrivileged | GatewayIntents.All,
    });

    ApplicationCommandService<ApplicationCommandContext> applicationCommandService = new();
    CommandService<CommandContext> commandService = new();
    applicationCommandService.AddModules(typeof(Program).Assembly);
    commandService.AddModules(typeof(Program).Assembly);

    client.MessageCreate += async message =>
    {
        if (!message.Content.StartsWith(prefix) || message.Author.IsBot)
            return;

        var result = await commandService.ExecuteAsync(prefixLength: 1, new CommandContext(message, client));

        if (result is not IFailResult failResult)
            return;

            try
        {
            await message.ReplyAsync(failResult.Message);
        }
        catch
        {
        }
    };

    client.InteractionCreate += async interaction =>
    {
        if (interaction is not ApplicationCommandInteraction applicationCommandInteraction)
            return;

        var result = await applicationCommandService.ExecuteAsync(new ApplicationCommandContext(applicationCommandInteraction, client));


        if (result is not IFailResult failResult)
            return;

        try
        {
            await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
        }
        catch { }
    };

    await applicationCommandService.RegisterCommandsAsync(client.Rest, client.Id);

    await client.StartAsync();
    await Task.Delay(-1);