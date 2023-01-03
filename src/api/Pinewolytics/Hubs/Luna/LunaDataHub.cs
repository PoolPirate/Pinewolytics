namespace Pinewolytics.Hubs.Luna;

public class LunaDataHub : DataHub<LunaDataClient, ILunaDataHubClient>
{
    public LunaDataHub(LunaDataClient client)
        : base(client)
    {
    }
}
