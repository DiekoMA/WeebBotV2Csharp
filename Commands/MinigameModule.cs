using System.ComponentModel.Design;
using System.Diagnostics;
using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace WeebBot;

public class MinigameModule : ApplicationCommandModule<ApplicationCommandContext>
{
    private string codeUrl = "https://renstudio.onrender.com/secretcode.txt";
    private readonly List<ulong> AllowedUsers = new List<ulong> { 708695852159467570, 582287188952350731, 456082475970461699,  350251280616128515, 561829188466769920 };
    private readonly ulong SecretChannel = 1482829370656821521;
    //private readonly string objectionTrack = @"C:\Users\Mayo\Documents\Projects\WeebBotV2\audio\Objection.mp3";
    private readonly string objectionTrack = @"C:\Users\Mayo\Downloads\Ace Attorney Objection.opus";
    [SlashCommand("trial", "Call a trial between 2 defendants and 1 witness")]
    public async Task CallTrial(User? accused = null, User? witness = null)
    {
        var guild = Context.Guild!;
        var caller = Context.User!;
        var client = Context.Client;
        var courtroomChannel = await Context.Client.Rest.GetChannelAsync(1489200905608757350);
        var testUser = await guild.GetUserAsync(350251280616128515);
        
        if (!caller.IsBot && !guild.VoiceStates.TryGetValue(caller.Id, out var voiceState))
        {
            await RespondAsync(InteractionCallback.Message("You need to be in a channel to run this command."));
            return;
        }

        
    }

    [SlashCommand("codeblack", "wouldn't you like to fucking know", DefaultGuildPermissions = Permissions.Administrator)]
    public async Task ExecuteCodeBlack()
    {
        var user = Context.User;
        var guild = Context.Guild;
        var client = Context.Client;
        if (!AllowedUsers.Contains(user.Id))
        {
            await RespondAsync(InteractionCallback.Message("You are not authorized to use this command."));
            return;
        }
        
        foreach (var id in AllowedUsers)
        {
            var allowedUser = await guild.GetUserAsync(id);
            
            if (allowedUser != null && allowedUser.GetVoiceStateAsync() != null)
            {
                await client.Rest.ModifyGuildUserAsync(guild.Id, allowedUser.Id, options =>
                {
                    options.ChannelId = SecretChannel;
                });
                await RespondAsync(InteractionCallback.Message($"Done."));
            } else
            {
                await RespondAsync(InteractionCallback.Message($"Some users were not moved cause they were online or they are already in the vc."));
            }
        }
    }

    [SlashCommand("self-distruct", "Initiate self destruct sequence")]
    public async Task SelfDistruct(int code)
    {
        
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(codeUrl);
            
            try
            {
                var response = await client.GetAsync($"/{code}"); // adjust endpoint as needed
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    // Parse the response - adjust based on your API format
                    if (int.TryParse(content.Trim(), out int correctCode))
                    {
                        if (code == correctCode)
                        {
                            await RespondAsync(InteractionCallback.Message("✅ Code verified! Self-destruct sequence initiated."));
                            // Do whatever you want on success
                        }
                        else
                        {
                            await RespondAsync(InteractionCallback.Message($"❌ Invalid code! You provided `{code}` but the correct code is `{correctCode}`"));
                        }
                    }
                    else
                    {
                        await RespondAsync(InteractionCallback.Message("Failed to parse code from endpoint"));
                    }
                }
                else
                {
                    await RespondAsync(InteractionCallback.Message($"Failed to fetch code: {response.StatusCode}"));
                }
            }
            catch (Exception ex)
            {
                await RespondAsync(InteractionCallback.Message($"Error: {ex.Message}"));
            }
        }
    }

    // [SlashCommand]

    // [SlashCommand("gamble", "")]
    // public async Task StartGambling(CasinoGame game)
    // {
    //     switch (game)
    //     {
    //         case CasinoGame.Slots:
    //         break;
            
    //         case CasinoGame.Roulette:
    //         break;
    //     }
    // }
}