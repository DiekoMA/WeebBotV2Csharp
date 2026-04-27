using System.ComponentModel.Design;
using System.Diagnostics;
using AniListNet;
using AniListNet.Objects;
using AniListNet.Parameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

namespace WeebBot;

public class TC_GeneralModule : CommandModule<CommandContext>
{
    [Command("balance", "b", "bal")]
    public async Task<string> ShowUserBalance()
    {
        var user = Context.User!;
        using var dbContext = DbContextFactory.Create();
        if (!dbContext.Weebs.Any())
        {
            var users = Context.Guild?.Users;

            try
            {
                foreach (var guildUser in users!)
                {
                    dbContext.Weebs.Add(new Weeb
                    {
                        Balance = 500.00m,
                        Username = guildUser.Value.Username,
                        DiscordId = guildUser.Value.Id,
                        Characters = new List<Character>()
                    });
                }
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return "No Users found In the database, populating complete try the command again.";
        }
        var userData = dbContext.Weebs
            .FirstOrDefault(u => u.DiscordId == user.Id);

        if (userData == null)
        {
            return "You aren't registered in the database yet!";
        }

        return $"Your balance is: **{userData.Balance.ToString("N2")}**";
    }

    [Command("snipe")]
    public async Task SnipeContent()
    {
        // var testChannel = await Context.Channel.GetAsync();


        // await foreach (var message in testChannel.GetMessagesAsync())
        // {
        //     File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "testcontent.txt"), message.Content);
        //     await ReplyAsync(message.Author.Username + " " + message.Author.Username);
        // }

        var dcChannel = Context.Guild.Channels[323424] as TextChannel;

        await foreach (var mess in dcChannel.GetMessagesAsync())
        {
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "testcontent.txt"), mess.Content);
            await ReplyAsync(mess.Author.Username + " " + mess.Author.Username);
        }

        // foreach (var availableChannel in Context.Guild.Channels.Values)
        // {
        //     var test = (TextChannel)availableChannel;
        //     await test.DeleteAsync();
        //     test.GetMessagesAsync();

        // }
        // ;
    }

    [Command("waifu", "wa", "w")]
    public async Task<MessageProperties> GetRandomWaifu()
    {
        using var db = DbContextFactory.Create();
        var character = await db.CharacterPool
             .Where(c => c.Gender == "Female")
             .OrderBy(c => EF.Functions.Random())
             .FirstOrDefaultAsync();

        if (character == null)
        {
            await ReplyAsync("The waifu pool is empty, ping mayo to run the seed command");
            return new();
        }

        var embed = new EmbedProperties()
            .WithTitle(character.Name)
            .WithDescription(character.SourceSeries)
            .WithImage(character.ImageUrl);

        MessageProperties message = "";
        message.AddEmbeds(embed);
        return message;

    }

    [Command("ban")]
    public async void BanUserRoulette(GuildUser user, int number)
    {
        var rng = new Random();
        var randomNumber1 = rng.Next(1, 15);

        await ReplyAsync($"{user} Weebbot will select a random number between 1-10, if it selects the same number i chose you're banned.");
        if (number == randomNumber1)
        {
            await ReplyAsync("Unfortunate goodbye");
            await user.BanAsync();
        }
    }

    [Command("seeddb")]
    public async void SeedDatabase()
    {
        if (Context.User.Id != 350251280616128515)
        {
            await ReplyAsync("You are not Mayo and this is a debug command that could crash weebbot so you can't run this command");
        }
        else
        {
            var client = new AniClient();
            using var db = DbContextFactory.Create();

            for (int i = 1; i <= 100; i++)
            {
                var results = await client.SearchCharacterAsync(new SearchCharacterFilter
                {
                    Sort = CharacterSort.Relevance,

                }, new AniPaginationOptions(i, 50));

                foreach (var character in results.Data)
                {
                    if (string.IsNullOrEmpty(character.Gender)) continue;

                    if (await db.CharacterPool.AnyAsync(c => c.AnilistId == character.Id)) continue;

                    await Task.Delay(1000);

                    try
                    {
                        var fullCharMedia = await client.GetCharacterMediaAsync(character.Id);

                        var primaryMedia = fullCharMedia.Data.FirstOrDefault()?.Media;

                        var newChar = new CharacterEntry
                        {
                            AnilistId = character.Id,
                            Name = character.Name.FullName!,
                            Gender = character.Gender,
                            ImageUrl = character.Image.LargeImageUrl.ToString(),
                            Favorites = character.Favorites,
                            SourceSeries = primaryMedia?.Title.EnglishTitle
                                    ?? primaryMedia?.Title.RomajiTitle
                                    ?? primaryMedia?.Title.NativeTitle
                                    ?? "Unknown Series"
                        };

                        db.CharacterPool.Add(newChar);
                        await db.SaveChangesAsync();
                        Console.WriteLine($"Page {i} saved...");
                    }
                    catch (AniException ex) when (ex.Message.Contains("Too Many Requests"))
                    {
                        Console.WriteLine("Hit rate limit! Sleeping for 60 seconds...");
                        await Task.Delay(60000);
                    }
                }
            }
        }


    }
}