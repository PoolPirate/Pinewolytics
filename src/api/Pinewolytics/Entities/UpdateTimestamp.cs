namespace Pinewolytics.Entities;

public class UpdateTimestamp
{
    public const string WalletRankingsKey = "Osmosis_Wallet_Rankings";

    public required string Key { get; set; }
    public required DateTimeOffset Timestamp { get; set; }

    public static UpdateTimestamp Now() => new UpdateTimestamp()
    {
        Key = null!,
        Timestamp = DateTimeOffset.UtcNow
    };
}
