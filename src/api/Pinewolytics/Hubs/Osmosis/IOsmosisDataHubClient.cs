using Pinewolytics.Models.DTOs.All;
using Pinewolytics.Models.DTOs.Osmosis;

namespace Pinewolytics.Hubs.Osmosis;

public interface IOsmosisDataHubClient
{
    Task TotalSupply(double totalSupply);
    Task CommunityPoolBalance(double communityPoolBalance);

    Task CurrentEpochInfo(OsmosisEpochInfoDTO epochInfoDto);
}
