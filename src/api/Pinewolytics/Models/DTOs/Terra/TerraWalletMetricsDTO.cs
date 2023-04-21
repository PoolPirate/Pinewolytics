namespace Pinewolytics.Models.DTOs.Terra;

public class TerraWalletMetricsDTO : IFlipsideObject<TerraWalletMetricsDTO>
{
    public required DateTimeOffset Timestamp { get; init; }

    public required long TotalReceivers { get; init; }
    public required long TotalSenders { get; init; }
    public required long TotalSendersAndReceivers { get; init; }

    public required long Senders { get; init; }
    public required long Receivers { get; init; }

    public static TerraWalletMetricsDTO Parse(string[] rawValues)
    {
        return new TerraWalletMetricsDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            TotalReceivers = long.Parse(rawValues[1]),
            TotalSenders = long.Parse(rawValues[2]),
            TotalSendersAndReceivers = long.Parse(rawValues[3]),
            Senders = long.Parse(rawValues[4]),
            Receivers = long.Parse(rawValues[5]),
        };
    }
}
