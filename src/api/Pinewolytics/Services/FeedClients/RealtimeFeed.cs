namespace Pinewolytics.Services.StreamClients;

[AttributeUsage(AttributeTargets.Method)]
public class RealtimeFeed : Attribute
{
    public string Key { get; }
    public int CacheSize { get; }

    public RealtimeFeed(string key, int cacheSize)
    {
        Key = key;
        CacheSize = cacheSize;
    }
}
