namespace Pinewolytics.Hubs.Osmosis;

public class OsmosisDataHub : DataHub<OsmosisDataClient, IOsmosisDataHubClient>
{
    public OsmosisDataHub(OsmosisDataClient client)
        : base(client)
    {
    }
}
