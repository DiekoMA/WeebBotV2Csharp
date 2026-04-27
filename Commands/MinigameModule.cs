using System.ComponentModel.Design;
using System.Data.SqlTypes;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
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
    public async Task SelfDistruct(string code)
    {
        var user = Context.User!;
        var guild = Context.Guild!;
        var client = Context.Client;
        using var db = DbContextFactory.Create();

        
        var userData = await db.Weebs.FirstOrDefaultAsync(u => u.DiscordId == user.Id);

        if (userData == null) return;

        var now = DateTimeOffset.UtcNow;
        if ((now - userData.LastSelfDestructTry).TotalHours < 24)
        {
            var remaining = TimeSpan.FromHours(24) - (now - userData.LastSelfDestructTry);
            await RespondAsync(InteractionCallback.Message($"You have no tries left for today You can try again in {remaining.Hours}h {remaining.Minutes}m."));
            return;
        }

        string dailyCode = Environment.GetEnvironmentVariable("SD_CODE") ?? GenerateDailyCode();
        Environment.SetEnvironmentVariable("SD_CODE", dailyCode);
        if (code != dailyCode)
        {
            userData.LastSelfDestructTry = now;
            await db.SaveChangesAsync();
            await RespondAsync(InteractionCallback.Message("Incorrect code, try again another day"));
            return;
        }

        var users = Context.Guild?.Users;

        try
        {
            foreach (var guildUser in users!)
            {
                foreach (var role in guildUser.Value.RoleIds)
                {
                    await guild.RemoveUserRoleAsync(guildUser.Value.Id, 1489054560755519610);
                }
                // await guild.KickUserAsync(guildUser.Value.Id);
                // await guild.BanUserAsync(guildUser.Value.Id);
            }
                
        } catch (Exception ex)
        {
            Console.WriteLine(ex);
        } 
    }

    [SlashCommand("reset-sd-code", "Generate a new self-destruct code", DefaultGuildPermissions = Permissions.Administrator)]
    public async Task ResetSdCode()
    {
        string newCode = GenerateFreshCode();
        Environment.SetEnvironmentVariable("SD_CODE", newCode);

        await RespondAsync(InteractionCallback.Message(
            $"✅ New self-destruct code generated and set: `{newCode}`"));
    }

    private string GenerateDailyCode()
    {
        int seed = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
        Random rng = new Random(seed);

        return rng.Next(100000, 999999).ToString();
    }

    private string GenerateFreshCode()
    {
        return new Random().Next(100000, 999999).ToString();
    }

    // private string GetOrGenerateDailyCode(Weeb userData, DateTimeOffset now)
    // {
    //     if (userData.DailyCode != null &&
    //         userData.DailyCodeGeneratedAt.Date == now.UtcDateTime.Date)
    //     {
    //         return userData.DailyCode;
    //     }

    //     // Otherwise generate a fresh one and save it
    //     int seed = int.Parse(now.ToString("yyyyMMdd"));
    //     var rng = new Random(seed);
    //     string newCode = rng.Next(100000, 999999).ToString();

    //     userData.DailyCode = newCode;
    //     userData.DailyCodeGeneratedAt = now;

    //     return newCode;
    // }

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