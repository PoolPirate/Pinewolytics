using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Database;
using Pinewolytics.Models.DTOs.Osmosis;
using Pinewolytics.Services;
using Pinewolytics.Utils;

namespace Pinewolytics.Controllers;

[Route("Api")]
[ApiController]
public class QueryController : ControllerBase
{
    private readonly QueryClient QueryClient;
    private readonly PinewolyticsContext DbContext;

    public QueryController(QueryClient queryClient, PinewolyticsContext dbContext)
    {
        QueryClient = queryClient;
        DbContext = dbContext;
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

    [HttpGet("Osmosis/Swaps")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetSwapsAsync(
        [FromQuery][ModelBinder(typeof(CommaDelimitedArrayModelBinder))] string[] addresses,
        CancellationToken cancellationToken)
    {
        var swaps = await QueryClient.GetOsmosisSwapsAsync(addresses, cancellationToken);
        return Ok(swaps);
    }

    [HttpGet("Osmosis/Transfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetTransfersAsync(
        [FromQuery][ModelBinder(typeof(CommaDelimitedArrayModelBinder))] string[] addresses,
        CancellationToken cancellationToken)
    {
        var transfers = await QueryClient.GetOsmoTransfersAsync(addresses, cancellationToken);
        return Ok(transfers);
    }

    [HttpGet("Osmosis/InternalNetTransfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetInternalNetOsmoTransfersAsync(
        [FromQuery][ModelBinder(typeof(CommaDelimitedArrayModelBinder))] string[] addresses,
        CancellationToken cancellationToken)
    {
        var transfers = await QueryClient.GetInternalNetOsmoTransfersAsync(addresses, cancellationToken);
        return Ok(transfers);
    }

    [HttpGet("Osmosis/ExternalNetTransfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetExternalNetOsmoTransfersAsync(
        [FromQuery][ModelBinder(typeof(CommaDelimitedArrayModelBinder))] string[] addresses,
        CancellationToken cancellationToken)
    {
        var transfers = await QueryClient.GetExternalNetOsmoTransfersAsync(addresses, cancellationToken);
        return Ok(transfers);
    }

    [HttpGet("Osmosis/IBCTransfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetIBCTransfersAync(
        [FromQuery][ModelBinder(typeof(CommaDelimitedArrayModelBinder))] string[] addresses,
        CancellationToken cancellationToken)
    {
        var ibcTransfers = await QueryClient.GetOsmoIBCTransfersAsync(addresses, cancellationToken);
        return Ok(ibcTransfers);
    }

    [HttpGet("Osmosis/LPJoins")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetLPJoinsAsync(
        [FromQuery][ModelBinder(typeof(CommaDelimitedArrayModelBinder))] string[] addresses,
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

    [HttpGet("Osmosis/RelatedWallets")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetDeveloperWalletsRecursiveAsync(
        [FromQuery][ModelBinder(typeof(CommaDelimitedArrayModelBinder))] string[] addresses,
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
        var ranking = await QueryClient.GetOsmosisWalletRankingAsync(address, cancellationToken);
        return Ok(ranking);
    }

    [HttpGet("Osmosis/PoolInfos")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "poolIds" })]
    public async Task<IActionResult> GetPoolInfosAsync([FromQuery][ModelBinder(typeof(CommaDelimitedArrayModelBinder))] uint[] poolIds,
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
