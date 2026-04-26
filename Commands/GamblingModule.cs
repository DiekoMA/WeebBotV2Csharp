using System.ComponentModel.Design;
using System.Diagnostics;
using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

public class GamblingModule : ApplicationCommandModule<ApplicationCommandContext>
{
    private SlotMachine machine = new SlotMachine();
    
    [SlashCommand("set-odds", "Set the odds of winning the jackpot")]
    public async Task SetOdds(OddsPercentage percentage)
    {
        if (Context.User.Id == 350251280616128515)
        {
            machine.JackpotChance = OddsHelper.ToDouble(percentage); 
                    machine.WinChance = OddsHelper.ToDouble(percentage); 
                    await RespondAsync(InteractionCallback.Message($"Jackpot odds set to {OddsHelper.ToDouble(percentage) * 100}%"));
                    await Context.Channel.SendMessageAsync($"Jackpot odds set to {OddsHelper.ToDouble(percentage) * 100}%");
        }
        else
        {
            await RespondAsync(InteractionCallback.Message("You don't have access to this command."));
        }
        
    }

    [SlashCommand("cv2", "Components V2")]
    public static InteractionMessageProperties CV2()
    {
        return new()
        {
            Components = [
                new MediaGalleryProperties().AddItems(new MediaGalleryItemProperties(new("https://netcord.dev/images/SmallSquare.png")),
                                                    new MediaGalleryItemProperties(new("https://netcord.dev/images/intents_Privileged.webp"))),
                new ComponentSeparatorProperties().WithSpacing(ComponentSeparatorSpacingSize.Large),
                new ComponentSectionProperties(new LinkButtonProperties("https://netcord.dev", "NetCord!")).AddComponents(new TextDisplayProperties("NetCord ➡️")),
            ],
            Flags = MessageFlags.IsComponentsV2,
        };
    }

    [SlashCommand("spin", "Spin the wheel and try your luck at winning the jackpot")]
    public async Task Spin()
    {
        SlotMachine machine = new SlotMachine();
        var result = machine.Spin();
        var slotEmbed = new EmbedProperties()
            .WithTitle("Slot Machine")
            .WithDescription("Dunno man, it's a slot machine, what else do you want.")
            .WithTimestamp(DateTimeOffset.UtcNow)
            .AddFields(
                new EmbedFieldProperties()
                    .WithName("Reel 1")
                    .WithValue($"{result.ReelEmojis[0]}")
                    .WithInline(true),

                new EmbedFieldProperties()
                    .WithName("Reel 2")
                    .WithValue($"{result.ReelEmojis[2]}")
                    .WithInline(true),

                new EmbedFieldProperties()
                    .WithName("Reel 3")
                    .WithValue($"{result.ReelEmojis[2]}")
                    .WithInline(true),

                new EmbedFieldProperties()
                    .WithName("Status")
                    .WithValue($"+{result.Winnings} coins")
                    .WithInline(false)
            );
        await RespondAsync(InteractionCallback.Message(new () {Embeds = new[] {slotEmbed}}));
    }

    [SlashCommand("balance", "Check your balance.")]
    public async Task CheckBalance()
    {
        
    }
}
