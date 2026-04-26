using System.Runtime.Serialization;

public enum CasinoGame
{
    Slots,
    Roulette
}

public enum OddsType
{
    Jackpot,
    Regular
}

public enum CoinFace
{
    Heads, 
    Tails
}

public enum OddsPercentage
{
    [EnumMember(Value ="10%")]
    Ten,
    [EnumMember(Value ="20%")]
    Twenty,
    [EnumMember(Value ="30%")]
    Thirty,
    [EnumMember(Value ="40%")]
    Forty,
    [EnumMember(Value ="50%")]
    Fifty,
    [EnumMember(Value ="60%")]
    Sixty,
    [EnumMember(Value ="70%")]
    Seventy,
    [EnumMember(Value ="80%")]
    Eighty,
    [EnumMember(Value ="90%")]
    Ninety,
    [EnumMember(Value ="100%")]
    Hundred,
}