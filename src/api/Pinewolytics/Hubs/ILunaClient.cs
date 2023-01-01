namespace Pinewolytics.Hubs;

public interface ILunaClient
{
    Task UpdatePrice(double price);
    Task UpdatePeakBlockInfo(ulong height, DateTimeOffset timestamp);
}
