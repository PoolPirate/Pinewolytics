namespace Pinewolytics.Models.DTOs.Terra;

public class OptimismWalletMetricsDTO : IFlipsideObject<OptimismWalletMetricsDTO>
{
    public required DateTimeOffset Timestamp { get; init; }

    public required long TotalReceivers { get; init; }
    public required long TotalSenders { get; init; }
    public required long TotalSendersAndReceivers { get; init; }

    public required long DailySenders { get; init; }
    public required long DailyReceivers { get; init; }

    public required long WeeklySenders { get; init; }
    public required long WeeklyReceivers { get; init; }

    public static OptimismWalletMetricsDTO Parse(string[] rawValues)
    {
        return new OptimismWalletMetricsDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            TotalReceivers = long.Parse(rawValues[1]),
            TotalSenders = long.Parse(rawValues[2]),
            TotalSendersAndReceivers = long.Parse(rawValues[3]),
            DailySenders = long.Parse(rawValues[4]),
            DailyReceivers = long.Parse(rawValues[5]),
            WeeklySenders = long.Parse(rawValues[6]),
            WeeklyReceivers = long.Parse(rawValues[7]),
        };
    }
}
