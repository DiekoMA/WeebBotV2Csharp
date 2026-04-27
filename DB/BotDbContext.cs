using Microsoft.EntityFrameworkCore;

public class BotDbContext : DbContext
{
    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options) { }
    public DbSet<Weeb> Weebs { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<CharacterEntry> CharacterPool { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CharacterEntry>().ToTable("character_pool");
        modelBuilder.Entity<Weeb>(entity =>
        {
            entity.ToTable("weebs");
            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.Username)
                .HasColumnName("username");
            entity.Property(e => e.DiscordId)
                .HasColumnName("discord_id");
            entity.Property(e => e.Balance)
                .HasColumnName("balance");
            entity.Property(e => e.LastSelfDestructTry)
                .HasColumnName("last_self_destruct_try");
        });
        
        
    }
}


public static class DbContextFactory
{
    private static string GetConnectionString()
    {
        var db_connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        if (string.IsNullOrEmpty(db_connectionString))
        {
            throw new Exception("CRITICAL: DB_CONNECTION_STRING environment variable is not set.");
        }

        return db_connectionString;
    }

    public static BotDbContext Create()
    {
        var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();
        optionsBuilder.UseNpgsql(GetConnectionString());

        return new BotDbContext(optionsBuilder.Options);
    }
}