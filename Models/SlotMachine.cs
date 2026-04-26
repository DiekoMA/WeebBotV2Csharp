public class SlotMachine
{
    private Random _random = new Random();
    public double JackpotChance {get; set; } = 0.01; 
    public double WinChance { get; set; } = 0.15; // 15% chance for regular win
    
    // Payouts
    public int JackpotAmount { get; set; } = 1000;
    public int WinAmount { get; set; } = 50;
    public int SpinCost { get; set; } = 10;

    private Dictionary<string, string> _symbolImages = new Dictionary<string, string>()
    {
        {"cherry", "<:slot_cherry:1491848028334719186>"},
        {"diamond", "<:slot_diamond:1491847584065786010>"},
        {"orange", "<:slot_orange:1491847722670755930>"},
        {"machine", "<:slot_machine:1491847698423484626>"},
        {"lemon", "<:slot_lemon:1491847676570894548>"},
    };

    private string[] _symbols = { "cherry", "diamond", "orange", "machine", "lemon" };
    public string GetSymbolEmoji(string symbol) => _symbolImages.ContainsKey(symbol) ? _symbolImages[symbol] : "";
    public SpinResult Spin()
    {
        var result = new SpinResult();

        result.Reels = new string[3];
        result.ReelEmojis = new string[3];

        for (int i = 0; i < 3; i++)
        {
            result.Reels[i] = _symbols[_random.Next(_symbols.Length)];
            result.ReelEmojis[i] = GetSymbolEmoji(result.Reels[i]);
        }

        double roll = _random.NextDouble();

        if (roll < JackpotChance)
        {
            result.IsJackpot = true;
            result.Winnings = JackpotAmount;
            result.Message = "  CONGRATS, YOU WON THE JACKPOT!";
        }
        else if (roll < JackpotChance + WinChance)
        {
            if (result.Reels[0] == result.Reels[1] || result.Reels[1] == result.Reels[2])
            {
                result.IsWin = true;
                result.Winnings = WinAmount;
                result.Message = "You won!";
            }
            else            {
                result.Message = "WOMP WOMP, you lost...";
            }
        }
        else
        {
            result.Message = "WOMP WOMP, you lost...";
        }
        return result;
    }
}