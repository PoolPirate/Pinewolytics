namespace Pinewolytics.Hubs;

public interface ILunaClient
{
    Task UpdatePrice(double price);
    Task UpdateBlockHeight(ulong blockHeight);
}
