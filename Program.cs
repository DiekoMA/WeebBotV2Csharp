using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;


    
GatewayClient client;
string token = Environment.GetEnvironmentVariable("BotToken") ?? "";

client = new GatewayClient(new BotToken(token), new GatewayClientConfiguration
{
    Logger = new ConsoleLogger(),
    Intents = GatewayIntents.All,
});

ApplicationCommandService<ApplicationCommandContext> applicationCommandService = new ();

applicationCommandService.AddModules(typeof(Program).Assembly);

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
    } catch {}
};

await applicationCommandService.RegisterCommandsAsync(client.Rest, client.Id);

await client.StartAsync();
await Task.Delay(-1);