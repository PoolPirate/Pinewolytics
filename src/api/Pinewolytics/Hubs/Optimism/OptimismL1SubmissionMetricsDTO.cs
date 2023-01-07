using Pinewolytics.Models;

namespace Pinewolytics.Hubs.Optimism;

public class OptimismL1SubmissionMetricsDTO : IFlipsideObject<OptimismL1SubmissionMetricsDTO>
{
    public required DateTimeOffset Timestamp { get; init; }

    public required ulong TotalSubmissions { get; init; }

    public static OptimismL1SubmissionMetricsDTO Parse(string[] rawValues)
    {
        return new OptimismL1SubmissionMetricsDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            TotalSubmissions = ulong.Parse(rawValues[1]),
        };
    }
}
