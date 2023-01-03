namespace Pinewolytics.Hubs.Luna;

public interface ILunaDataHubClient
{
    Task Price(double price);
    Task PeakBlockHeight(ulong height);
    Task PeakBlockTimestamp(DateTimeOffset timestamp);
}
