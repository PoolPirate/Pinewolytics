﻿using Microsoft.AspNetCore.Mvc;
using Pinewolytics.Services;

namespace Pinewolytics.Controllers;

[Route("Api")]
[ApiController]
public class QueryController : ControllerBase
{
    private readonly QueryClient QueryClient;

    public QueryController(QueryClient queryClient)
    {
        QueryClient = queryClient;
    }

    [HttpGet("Osmosis/Swaps")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetSwapsAsync([FromQuery] string[] addresses, 
        CancellationToken cancellationToken)
    {
        var swaps = await QueryClient.GetOsmosisSwapsAsync(addresses, cancellationToken);
        return Ok(swaps);
    }

    [HttpGet("Osmosis/Transfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetTransfersAsync([FromQuery] string[] addresses, 
        CancellationToken cancellationToken)
    {
        var transfers = await QueryClient.GetOsmoTransfersAsync(addresses, cancellationToken);
        return Ok(transfers);
    }

    [HttpGet("Osmosis/InternalNetTransfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetInternalNetOsmoTransfersAsync([FromQuery] string[] addresses, 
        CancellationToken cancellationToken)
    {
        var transfers = await QueryClient.GetInternalNetOsmoTransfersAsync(addresses, cancellationToken);
        return Ok(transfers);
    }

    [HttpGet("Osmosis/ExternalNetTransfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetExternalNetOsmoTransfersAsync([FromQuery] string[] addresses, 
        CancellationToken cancellationToken)
    {
        var transfers = await QueryClient.GetExternalNetOsmoTransfersAsync(addresses, cancellationToken);
        return Ok(transfers);
    }

    [HttpGet("Osmosis/IBCTransfers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetIBCTransfersAync([FromQuery] string[] addresses, 
        CancellationToken cancellationToken)
    {
        var ibcTransfers = await QueryClient.GetOsmoIBCTransfersAsync(addresses, cancellationToken);
        return Ok(ibcTransfers);
    }

    [HttpGet("Osmosis/LPJoins")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "addresses" })]
    public async Task<IActionResult> GetLPJoinsAsync([FromQuery] string[] addresses, 
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
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] {"depth"})]
    public async Task<IActionResult> GetDeveloperWalletsRecursiveAsync([FromQuery] int depth,
        CancellationToken cancellationToken)
    {
        var wallets = await QueryClient.GetDeveloperWalletsRecursiveAsync(depth, cancellationToken);
        return Ok(wallets);
    }

    [HttpGet("Osmosis/RelatedWallets")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] {"addresses"})]
    public async Task<IActionResult> GetDeveloperWalletsRecursiveAsync([FromQuery] string[] addresses,
    CancellationToken cancellationToken)
    {
        var wallets = await QueryClient.GetRelatedAddressesAsync(addresses, cancellationToken);
        return base.Ok((object)wallets);
    }
}
