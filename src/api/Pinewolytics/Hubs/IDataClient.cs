namespace Pinewolytics.Hubs;

public interface IDataClient
{
    public Task SendWelcomeToAsync(string connectionId);
}
