using Common.Services;
using Pinewolytics.Models.DTOs.Osmosis.ProtoRev;
using Pinewolytics.Services.ApiClients;
using Pinewolytics.Services.StreamClients;

namespace Pinewolytics.Services.FeedClients;

public class OsmosisFeedClient : BaseFeedClient
{
    [Inject]
    private readonly OsmosisRPCClient OsmosisRPC = null!;

    [RealtimeFeed("Osmosis-ProtoRev-Tx-Feed", maximumItemAgeSeconds: 1800)]
    public async IAsyncEnumerable<OsmosisProtoRevTransactionDTO> ProtoRevTXFeed()
    {
        ulong maxHeight = await OsmosisRPC.GetPeakBlockHeightAsync() - 500;
        int maxIndex = 0;
        int limit = 50;

        while (true)
        {
            OsmosisProtoRevTransactionDTO[] newTxs = null!;

            try
            {
                var txs = await OsmosisRPC.GetProtoRevTXsAsync(maxHeight, limit);

                if (txs.Length == limit && txs.All(x => x.Height == maxHeight))
                {
                    Logger.LogInformation("Osmosis-ProtoRev-Tx-Feed: Increase tx limit to {limit}", limit);
                    limit++; //Fix getting stuck on a block
                    await Task.Delay(10000);
                    continue;
                }

                newTxs = txs
                    .Where(x => x.Height > maxHeight || x.Index > maxIndex)
                    .ToArray();

                if (!newTxs.Any())
                {
                    await Task.Delay(10000);
                    continue;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Osmosis-ProtoRev-Tx-Feed: Feed refresh failed");
                await Task.Delay(10000);
                continue;
            }

            maxHeight = newTxs.Max(x => x.Height);
            maxIndex = newTxs.Max(x => x.Index);

            foreach (var tx in newTxs)
            {
                yield return tx;
            }

            await Task.Delay(10000);
        }
    }
}
