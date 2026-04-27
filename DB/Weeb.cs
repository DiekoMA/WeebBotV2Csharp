using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Weeb
{
    public int Id { get; set; }
    public string? Username { get; set; } = string.Empty;
    public ulong DiscordId { get; set; }
    
    public decimal Balance { get; set; }

    public DateTimeOffset LastSelfDestructTry { get; set; }


    public List<Character> Characters { get; set; } = new();
}

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int OwnerId { get; set; }
    public Weeb Owner { get; set; } = null!;
}

public class CharacterEntry
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("anilist_id")]
    public int AnilistId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("gender")]
    public string? Gender { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("favorites")]
    public int Favorites { get; set; }

    [Column("source_series")]
    public string? SourceSeries { get; set; }
}

