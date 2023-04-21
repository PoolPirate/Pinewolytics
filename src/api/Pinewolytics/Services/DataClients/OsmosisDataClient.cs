using Common.Services;
using Pinewolytics.Models.DTOs;
using Pinewolytics.Models.DTOs.Osmosis;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Services.DataClients;

public class OsmosisDataClient : BaseDataClient
{
    private const string CommunityPoolAddress = "osmo1jv65s3grqf6v6jl3dp4t6c9t9rk99cd80yhvld";

    [Inject]
    private readonly OsmosisLCDClient OsmosisLCD = null!;
    [Inject]
    private readonly OsmosisHistoricalDataClient OsmosisHistoricalData = null!;

    [RealtimeValue("Osmosis-Total-Supply", 300 * SECONDS)]
    public async Task<decimal> LoadPriceAsync()
    {
        return await OsmosisLCD.GetTotalOSMOSupplyAsync();
    }

    [RealtimeValue("Osmosis-Community-Pool-Balance", 150 * SECONDS)]
    public async Task<decimal> LoadCommunityPoolBalanceAsync()
    {
        return await OsmosisLCD.GetCurrentOSMOBalanceAsync(CommunityPoolAddress, default);
    }

    [RealtimeValue("Osmosis-Epoch-Info", 600 * SECONDS)]
    public async Task<OsmosisEpochInfoDTO> LoadCurrentEpochInfoAsync()
    {
        var epochs = await OsmosisLCD.GetEpochInfosAsync(default);
        return epochs.Single(x => x.Identifier == "day");
    }

    [RealtimeValue("Osmosis-Total-Superfluid-Delegations", 60 * SECONDS)]
    public async Task<double> LoadTotalSuperfluidDelegations()
    {
        return await OsmosisLCD.GetTotalSuperfluidDelegationsAsync(default);
    }

    [RealtimeValue("Osmosis-ProtoRev-Total-Revenue", 40 * SECONDS)]
    public async Task<DenominatedAmountDTO[]> LoadTotalProtoRevRevenueAsync()
    {
        return await OsmosisLCD.GetTotalProtoRevProfitsAsync(default);
    }

    [RealtimeValue("Osmosis-ProtoRev-Total-Trade-Count", 40 * SECONDS)]
    public async Task<long> LoadTotalProtoRevTradesAsync()
    {
        return await OsmosisLCD.GetTotalProtoRevTradeCountAsync();
    }

    [RealtimeValue("Osmosis-ProtoRev-Enabled", 300 * SECONDS)]
    public async Task<bool> LoadOsmosisProtoRevIsEnabledAsync()
    {
        return await OsmosisLCD.GetProtoRevIsEnabledAsync();
    }

    [RealtimeValue("Osmosis-ProtoRev-Admin-Address", 600 * SECONDS)]
    public async Task<string> LoadOsmosisProtoRevAdminAddressAsync()
    {
        return await OsmosisLCD.GetProtoRevAdminAddressAsync();
    }

    [RealtimeValue("Osmosis-ProtoRev-Developer-Address", 300 * SECONDS)]
    public async Task<string> LoadOsmosisProtoRevDeveloperAddressAsync()
    {
        return await OsmosisLCD.GetProtoRevDeveloperAddressAsync();
    }

    [RealtimeValue("Osmosis-ProtoRev-All-Route-Statistics", 300 * SECONDS)]
    public async Task<OsmosisProtoRevRouteStatisticsDTO[]> LoadOsmosisProtoRevRouteStatisticsAsync()
    {
        return await OsmosisLCD.GetProtoRevAllRouteStatisticsAsync();
    }

    [RealtimeValue("Osmosis-All-Token-Infos", 30 * SECONDS)]
    public async Task<OsmosisTokenInfoDTO[]> LoadAllOsmosisTokenInfosAsync()
    {
        return await OsmosisHistoricalData.GetAllTokenInfosAsync();
    }

    [RealtimeValue("Osmosis-ProtoRev-Module-Balance", 30 * SECONDS)]
    public async Task<DenominatedAmountDTO[]> LoadProtoRevModuleBalanceAsync()
    {
        return await OsmosisLCD.GetProtoRevModuleBalanceAsync();
    }
}
