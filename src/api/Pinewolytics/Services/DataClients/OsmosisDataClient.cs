using Common.Services;
using Pinewolytics.Models.DTOs.Osmosis;
using Pinewolytics.Services.ApiClients;
using static Pinewolytics.Services.ApiClients.OsmosisLCDClient;

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

    [RealtimeValue("Osmosis-ProtoRev-Total-Revenue", 40 * SECONDS)]
    private async Task<DenominatedAmount[]> LoadTotalProtoRevRevenueAsync()
    {
        return await OsmosisLCD.GetTotalProtoRevProfitsAsync(default);
    }

    [RealtimeValue("Osmosis-ProtoRev-Total-Trade-Count", 40 * SECONDS)]
    private async Task<long> LoadTotalProtoRevTradesAsync()
    {
        return await OsmosisLCD.GetTotalProtoRevTradeCountAsync();
    }

    [RealtimeValue("Osmosis-ProtoRev-Enabled", 300 * SECONDS)]
    private async Task<bool> LoadOsmosisProtoRevIsEnabledAsync()
    {
        return await OsmosisLCD.GetProtoRevIsEnabledAsync();
    }

    [RealtimeValue("Osmosis-ProtoRev-Admin-Address", 600 * SECONDS)]
    private async Task<string> LoadOsmosisProtoRevAdminAddressAsync()
    {
        return await OsmosisLCD.GetProtoRevAdminAddressAsync();
    }

    [RealtimeValue("Osmosis-ProtoRev-Developer-Address", 300 * SECONDS)]
    private async Task<string> LoadOsmosisProtoRevDeveloperAddressAsync()
    {
        return await OsmosisLCD.GetProtoRevDeveloperAddressAsync();
    }
}
