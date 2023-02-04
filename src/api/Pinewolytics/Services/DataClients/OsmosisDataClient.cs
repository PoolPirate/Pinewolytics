using Common.Services;
using Pinewolytics.Models.DTOs.Osmosis;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Services.DataClients;

public class OsmosisDataClient : BaseDataClient
{
    private const string CommunityPoolAddress = "osmo1jv65s3grqf6v6jl3dp4t6c9t9rk99cd80yhvld";

    [Inject]
    private readonly OsmosisLCDClient OsmosisLCD = null!;

    [RealtimeValue("Osmosis-Total-Supply", 300 * SECONDS)]
    private async Task<double> LoadPriceAsync()
    {
        return await OsmosisLCD.GetTotalOSMOSupplyAsync(default);
    }

    [RealtimeValue("Osmosis-Community-Pool-Balance", 150 * SECONDS)]
    private async Task<double> LoadCommunityPoolBalanceAsync()
    {
        return await OsmosisLCD.GetCurrentOSMOBalanceAsync(CommunityPoolAddress, default);
    }

    [RealtimeValue("Osmosis-Epoch-Info", 600 * SECONDS)]
    private async Task<OsmosisEpochInfoDTO> LoadCurrentEpochInfoAsync()
    {
        var epochs = await OsmosisLCD.GetEpochInfosAsync(default);
        return epochs.Single(x => x.Identifier == "day");
    }

    [RealtimeValue("Osmosis-Total-Superfluid-Delegations", 60 * SECONDS)]
    private async Task<double> LoadTotalSuperfluidDelegations()
    {
        return await OsmosisLCD.GetTotalSuperfluidDelegationsAsync(default);
    }
}
