namespace Pinewolytics.Models.DTOs;

public class OsmosisFlowSankeyDTO : IFlipsideObject<OsmosisFlowSankeyDTO>
{
    public required ushort Decimals { get; init; }

    public required double NetSwapOut { get; init; }
    public required double NetSwapIn { get; init; }

    public required double NetLPDeposit { get; init; }
    public required double NetLPExit { get; init; }

    public required double NetIBCIn { get; init; }
    public required double NetIBCOut { get; init; }

    public required double NetTransferIn { get; init; }
    public required double NetTransferOut { get; init; }

    public required double NetStakingRewards { get; init; }

    public required double NetStaked { get; init; }
    public required double NetUnstaked { get; init; }

    public static OsmosisFlowSankeyDTO Parse(string[] rawValues)
    {
        var div = 1_000_000;

        return new OsmosisFlowSankeyDTO()
        {
            Decimals = ushort.Parse(rawValues[0]),
            NetSwapOut = double.Parse(rawValues[1]) / div,
            NetSwapIn = double.Parse(rawValues[2]) / div,
            NetLPDeposit = double.Parse(rawValues[3]) / div,
            NetLPExit = double.Parse(rawValues[4]) / div,
            NetIBCIn = double.Parse(rawValues[5]) / div,
            NetIBCOut = double.Parse(rawValues[6]) / div,
            NetTransferIn = double.Parse(rawValues[7]) / div,
            NetTransferOut = double.Parse(rawValues[8]) / div,
            NetStakingRewards = double.Parse(rawValues[9]) / div,
            NetStaked = double.Parse(rawValues[10]) / div,
            NetUnstaked = double.Parse(rawValues[11]) / div
        };
    }
}
