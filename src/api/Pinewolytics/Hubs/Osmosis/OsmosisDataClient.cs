using Common.Services;
using Pinewolytics.Models.DTOs.Osmosis;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Hubs.Osmosis;

public class OsmosisDataClient : BaseDataClient<OsmosisDataHub, IOsmosisDataHubClient>
{
    private const string CommunityPoolAddress = "osmo1jv65s3grqf6v6jl3dp4t6c9t9rk99cd80yhvld";

    [Inject]
    private readonly OsmosisLCDClient OsmosisLCD;

    [RealtimeValue(300 * SECONDS, nameof(IOsmosisDataHubClient.TotalSupply))]
    private async Task<double> LoadPriceAsync()
    {
        return await OsmosisLCD.GetTotalOSMOSupplyAsync(default);
    }

    [RealtimeValue(150 * SECONDS, nameof(IOsmosisDataHubClient.CommunityPoolBalance))]
    private async Task<double> LoadCommunityPoolBalanceAsync()
    {
        return await OsmosisLCD.GetCurrentOSMOBalanceAsync(CommunityPoolAddress, default);
    }

    [RealtimeValue(600 * SECONDS, nameof(IOsmosisDataHubClient.CurrentEpochInfo))]
    private async Task<OsmosisEpochInfoDTO> LoadCurrentEpochInfoAsync()
    {
        var epochs = await OsmosisLCD.GetEpochInfosAsync(default);
        return epochs.Single(x => x.Identifier == "day");
    }
}
