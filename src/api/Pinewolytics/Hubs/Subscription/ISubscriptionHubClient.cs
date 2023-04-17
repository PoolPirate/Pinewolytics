namespace Pinewolytics.Hubs;

public interface ISubscriptionHubClient
{
    public Task SendQueryResult(string name, object result);
    public Task SendRealtimeValue(string name, object result);
    public Task SendRealtimeFeedExtension(string name, object result);
}
