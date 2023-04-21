using System.Globalization;

namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisProtoRevAssetRevenueDTO : IFlipsideObject<OsmosisProtoRevAssetRevenueDTO>
{
    public required string Currency { get; init; }
    public required ulong TotalAsset { get; init; }
    public required decimal TotalUSD { get; init; }

    public static OsmosisProtoRevAssetRevenueDTO Parse(string[] rawValues)
    {
        return new OsmosisProtoRevAssetRevenueDTO()
        {
            Currency = rawValues[0],
            TotalAsset = ulong.Parse(rawValues[1]),
            TotalUSD = decimal.Parse(rawValues[2], NumberStyles.Float)
        };
    }
}
