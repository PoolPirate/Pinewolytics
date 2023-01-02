namespace Pinewolytics.Hubs;

public interface ILunaDataHubClient
{
    Task UpdatePrice(double price);
    Task UpdatePeakBlockInfo(ulong height, DateTimeOffset timestamp);
}
