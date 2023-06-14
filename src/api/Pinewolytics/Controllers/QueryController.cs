using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Database;
using Pinewolytics.Entities;
using Pinewolytics.Models.DTOs.Osmosis;
using Pinewolytics.Services;
using Pinewolytics.Services.ApiClients;
using Pinewolytics.Utils;

namespace Pinewolytics.Controllers;

[Route("Api")]
[ApiController]
public class QueryController : ControllerBase
{
    private readonly QueryClient QueryClient;
    private readonly PinewolyticsContext DbContext;
    private readonly OsmosisLCDClient OsmosisLCDC;

    public QueryController(QueryClient queryClient, PinewolyticsContext dbContext, OsmosisLCDClient osmosisLCDC)
    {
        QueryClient = queryClient;
        DbContext = dbContext;
        OsmosisLCDC = osmosisLCDC;
    }

    [HttpGet("Query/Src/{queryName}")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetQuerySrcAsync([FromRoute] string queryName)
    {
        string? src = await QueryClient.GetQuerySrcAsync(queryName);

        return src is null
            ? NotFound()
            : Ok(new { src });
    }

    [HttpPost("Osmosis/Swaps")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetSwapsAsync(
        [FromBody] string[] addresses,
        CancellationToken cancellationToken)
    {
        var swaps = await QueryClient.GetOsmosisSwapsAsync(addresses, cancellationToken);
        return Ok(swaps);
    }

    [HttpPost("Osmosis/Transfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetTransfersAsync(
        [FromBody] string[] addresses, 
        CancellationToken cancellationToken)
    {
        var transfers = await QueryClient.GetOsmoTransfersAsync(addresses, cancellationToken);
        return Ok(transfers);
    }

    [HttpPost("Osmosis/InternalNetTransfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetInternalNetOsmoTransfersAsync(
        [FromBody] string[] addresses,
        CancellationToken cancellationToken)
    {
        var transfers = await QueryClient.GetInternalNetOsmoTransfersAsync(addresses, cancellationToken);
        return Ok(transfers);
    }

    [HttpPost("Osmosis/ExternalNetTransfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetExternalNetOsmoTransfersAsync(
         [FromBody] string[] addresses,
        CancellationToken cancellationToken)
    {
        var transfers = await QueryClient.GetExternalNetOsmoTransfersAsync(addresses, cancellationToken);
        return Ok(transfers);
    }

    [HttpPost("Osmosis/IBCTransfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetIBCTransfersAync(
        [FromBody] string[] addresses, 
        CancellationToken cancellationToken)
    {
        var ibcTransfers = await QueryClient.GetOsmoIBCTransfersAsync(addresses, cancellationToken);
        return Ok(ibcTransfers);
    }

    [HttpPost("Osmosis/LPJoins")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetLPJoinsAsync(
        [FromBody] string[] addresses,
        CancellationToken cancellationToken)
    {
        var lpJoins = await QueryClient.GetOsmoLPJoinsAsync(addresses, cancellationToken);
        return Ok(lpJoins);
    }

    [HttpGet("Osmosis/FlowSankey")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "address", "currency" })]
    public async Task<IActionResult> GetFlowSankeyAsync([FromQuery] string address, [FromQuery] string currency,
        CancellationToken cancellationToken)
    {
        var sankey = await QueryClient.GetOsmosisFlowSankeyDataAsync(address, currency, cancellationToken);
        return Ok(sankey);
    }

    [HttpGet("Osmosis/DeveloperWallets")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "depth" })]
    public async Task<IActionResult> GetDeveloperWalletsRecursiveAsync([FromQuery] int depth,
        CancellationToken cancellationToken)
    {
        string[] wallets = await QueryClient.GetDeveloperWalletsRecursiveAsync(depth, cancellationToken);
        return Ok(wallets);
    }

    [HttpPost("Osmosis/RelatedWallets")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetDeveloperWalletsRecursiveAsync(
        [FromBody] string[] addresses,
        CancellationToken cancellationToken)
    {
        string[] wallets = await QueryClient.GetRelatedAddressesAsync(addresses, cancellationToken);
        return Ok(wallets);
    }

    [HttpGet("Osmosis/WalletRankings/{address}")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "address" })]
    public async Task<IActionResult> GetWalletRankingAsync([FromRoute] string address,
        CancellationToken cancellationToken)
    {
        var ranking = await DbContext.WalletRankings
            .Include(x => x.LPerRanks)
            .Where(x => x.Address.ToUpper() == address.ToUpper())
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);

        if (ranking is null)
        {
            return NotFound();
        }

        var peak = await DbContext.UpdateTimestamps
            .Where(x => x.Key == UpdateTimestamp.WalletRankingsKey)
            .SingleOrDefaultAsync(cancellationToken: cancellationToken)
            ?? UpdateTimestamp.Now();

        return Ok(new OsmosisWalletRankingDTO()
        {
            Address = ranking.Address,
            StakedRank = ranking.StakedRank,
            StakedAmount = ranking.StakedAmount,
            BalanceRank = ranking.BalanceRank,
            BalanceAmount = await OsmosisLCDC.GetCurrentOSMOBalanceAsync(address, cancellationToken) ,
            LastUpdatedAt = peak.Timestamp,
            PoolRankings = ranking.LPerRanks!.Select(x => new OsmosisWalletPoolRankingDTO()
            {
                PoolId = x.PoolId,
                LPTokenBalance = x.GammBalance,
                Rank = x.Rank
            }).ToArray()
        });
    }

    [HttpGet("Osmosis/PoolInfos")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "poolIds" })]
    public async Task<IActionResult> GetPoolInfosAsync([FromQuery][ModelBinder(typeof(CommaDelimitedArrayModelBinder))] int[] poolIds,
    CancellationToken cancellationToken)
    {
        if (poolIds.Length == 0)
        {
            return Ok(Array.Empty<OsmosisPoolInfoDTO>());
        }

        

        var ranking = await QueryClient.GetOsmosisPoolInfosAsync(poolIds, cancellationToken);
        return Ok(ranking);
    }

    [HttpGet("Osmosis/ProtoRevTx/{address}")]
    [ResponseCache(Duration = 180, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "address" })]
    public async Task<IActionResult> GetProtoRevTxAsync([FromRoute] string address)
    {
        var swaps = await DbContext.ProtoRevTransactions
            .Include(x => x.Swaps)
            .Where(x => x.TxFrom == address)
            .ToArrayAsync();

        return Ok(
            swaps.Select(x => new OsmosisProtoRevTransactionDTO()
            {
                Hash= x.Hash,
                Timestamp = x.TimeStamp,
                TxFrom = x.TxFrom,
                Swaps = x.Swaps.Select(x => new OsmosisProtoRevSwapDTO()
                {
                    Profit = new Models.DTOs.DenominatedAmountDTO()
                    {
                        Denom = x.Currency,
                        Amount = x.Profit
                    },
                    ProfitUSD = x.ProfitUSD
                }).ToArray()
            })
        );
    }
}
