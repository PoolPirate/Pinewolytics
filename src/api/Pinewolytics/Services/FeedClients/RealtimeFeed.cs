namespace Pinewolytics.Services.StreamClients;

[AttributeUsage(AttributeTargets.Method)]
public class RealtimeFeed : Attribute
{
    public string Key { get; }
    public int CacheSizeLimit { get; }
    public TimeSpan? MaximumItemAge { get; }

    public RealtimeFeed(string key, int cacheSizeLimit = 0, int maximumItemAgeSeconds = 0)
    {
        Key = key;
        CacheSizeLimit = cacheSizeLimit;
        MaximumItemAge = maximumItemAgeSeconds == 0 
            ? null 
            : TimeSpan.FromSeconds(maximumItemAgeSeconds);
    }
}
