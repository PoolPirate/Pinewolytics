namespace Pinewolytics.Hubs;

public interface IQueryHubClient
{
    public Task SendQueryResult(string name, object[] result);
}
