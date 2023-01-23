using Common.Services;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Database;
using Pinewolytics.Models;
using Pinewolytics.Models.DTOs.Osmosis;
using Pinewolytics.Queries.Flipside;
using Pinewolytics.Services.ApiClients;
using System.Text;

namespace Pinewolytics.Services;

public class QueryClient : Singleton
{
    [Inject]
    private readonly FlipsideClient Flipside = null!;

    [Inject]
    private readonly OsmosisLCDClient OsmosisLCD = null!;

    public async Task<string?> GetQuerySrcAsync(string queryName)
    {
        using var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PinewolyticsContext>();

        var query = await dbContext.ScheduledQueries.SingleOrDefaultAsync(x => x.Name == queryName);
        return query?.Query;
    }

    public async Task<OsmosisSwapDTO[]> GetOsmosisSwapsAsync(string[] addresses, CancellationToken cancellationToken)
    {
        if (addresses.Length == 0)
        {
            return Array.Empty<OsmosisSwapDTO>();
        }

        string sql =
        $"""
         WITH scope AS (
        {string.Join(" UNION\n", addresses.Select(address => $"SELECT '{address}' AS address"))}
        ),
        swaps AS (
          SELECT *
          FROM osmosis.core.fact_swaps
          WHERE trader IN (SELECT * from scope)
        )

        SELECT block_timestamp, trader, 
               from_amount / POW(10, COALESCE(from_decimal, 0)) AS from_amount, from_currency,
               to_amount / POW(10, COALESCE(to_decimal, 0)) AS to_amount, to_currency
        FROM swaps
        
        """;

        return await Flipside.GetOrRunAsync<OsmosisSwapDTO>(sql, cancellationToken: cancellationToken);
    }

    public async Task<OsmosisTransferDTO[]> GetOsmoTransfersAsync(string[] addresses, CancellationToken cancellationToken)
    {
        if (addresses.Length == 0)
        {
            return Array.Empty<OsmosisTransferDTO>();
        }

        string sql =
        $"""
        WITH scope AS (
        {string.Join(" UNION\n", addresses.Select(address => $"SELECT '{address}' AS address"))}
        ),
        transfers AS (
           SELECT *
           FROM osmosis.core.fact_transfers
           WHERE (lower(sender) IN (SELECT lower(address) FROM scope) OR lower(receiver) IN (SELECT lower(address) FROM scope))
             AND (lower(sender) NOT IN (SELECT lower(address) FROM scope) OR lower(receiver) NOT IN (SELECT lower(address) FROM scope))
             AND currency = 'uosmo' AND transfer_type = 'OSMOSIS'
             AND amount >= POW(10, 5)
        )

        SELECT block_timestamp, 
               amount / POW(10, 6) AS amount,
               receiver, sender
        FROM transfers
        """;

        return await Flipside.GetOrRunAsync<OsmosisTransferDTO>(sql, cancellationToken: cancellationToken);
    }

    public async Task<OsmosisNetTransferDTO[]> GetInternalNetOsmoTransfersAsync(string[] addresses, CancellationToken cancellationToken)
    {
        if (addresses.Length == 0)
        {
            return Array.Empty<OsmosisNetTransferDTO>();
        }

        string sql = FlipsideQueries.InternalNetOSMOTransfers(addresses);
        return await Flipside.GetOrRunAsync<OsmosisNetTransferDTO>(sql, cancellationToken: cancellationToken);
    }

    public async Task<OsmosisNetTransferDTO[]> GetExternalNetOsmoTransfersAsync(string[] addresses, CancellationToken cancellationToken)
    {
        if (addresses.Length == 0)
        {
            return Array.Empty<OsmosisNetTransferDTO>();
        }

        string sql = FlipsideQueries.ExternalNetOSMOTransfers(addresses);
        return await Flipside.GetOrRunAsync<OsmosisNetTransferDTO>(sql, cancellationToken: cancellationToken);
    }

    public async Task<OsmosisIBCTransferDTO[]> GetOsmoIBCTransfersAsync(string[] addresses, CancellationToken cancellationToken)
    {
        if (addresses.Length == 0)
        {
            return Array.Empty<OsmosisIBCTransferDTO>();
        }

        string sql =
        $"""
        WITH scope AS (
        {string.Join(" UNION\n", addresses.Select(address => $"SELECT '{address}' AS address"))}
        ),
        transfers AS (
          SELECT *
          FROM osmosis.core.fact_transfers
          WHERE
            (
              transfer_type = 'IBC_TRANSFER_IN'
              OR transfer_type = 'IBC_TRANSFER_OUT'
            )
            AND currency = 'uosmo'
            AND amount >= POW(10, 5)
          )

        SELECT DATE_TRUNC('day', block_timestamp) AS hour,
               sum(amount) / POW(10, 6) AS amount, receiver, sender, transfer_type
        FROM transfers
        GROUP BY hour, receiver, sender, transfer_type
        """; //ToDo: Pagination

        Console.WriteLine(sql);

        return await Flipside.GetOrRunAsync<OsmosisIBCTransferDTO>(sql, cancellationToken: cancellationToken);
    }

    public async Task<OsmosisLPJoinDTO[]> GetOsmoLPJoinsAsync(string[] addresses, CancellationToken cancellationToken)
    {
        if (addresses.Length == 0)
        {
            return Array.Empty<OsmosisLPJoinDTO>();
        }

        string sql =
        $"""
        WITH scope AS (
        {string.Join(" UNION\n", addresses.Select(address => $"SELECT '{address}' AS address"))}
        ),
        lp_joins AS (
          SELECT *
          FROM osmosis.core.fact_liquidity_provider_actions
          WHERE action = 'pool_joined' AND currency = 'uosmo'
            AND lower(liquidity_provider_address) IN (SELECT lower(address) FROM scope)
            AND amount >= POW(10, 5)
        )

        SELECT block_timestamp, 
          liquidity_provider_address,
          amount / POW(10, 6) AS amount
        FROM lp_joins
        """;

        return await Flipside.GetOrRunAsync<OsmosisLPJoinDTO>(sql, cancellationToken: cancellationToken);
    }

    public async Task<OsmosisFlowSankeyDTO> GetOsmosisFlowSankeyDataAsync(string address, string currency, CancellationToken cancellationToken)
    {
        string sql =
        $"""
        WITH swaps AS (
          SELECT *
          FROM osmosis.core.fact_swaps
          WHERE trader = lower('{address}') 
            AND (lower(from_currency) = lower('{currency}') OR lower(to_currency) = lower('{currency}'))
        ),
        lpActions AS (
          SELECT *
          FROM osmosis.core.fact_liquidity_provider_actions
          WHERE liquidity_provider_address = lower('{address}')
            AND (action = 'pool_joined' OR action = 'pool_exited')
            AND lower(currency) = lower('{currency}')
        ),
        ibcTransfers AS (
          SELECT *
          FROM osmosis.core.fact_transfers
          WHERE (sender = lower('{address}') OR receiver = lower('{address}'))
            AND lower(currency) = lower('{currency}')
            AND (transfer_type = 'IBC_TRANSFER_IN' OR transfer_type = 'IBC_TRANSFER_OUT')
        ),
        transfers AS (
          SELECT *
          FROM osmosis.core.fact_transfers
          WHERE (sender = lower('{address}') OR receiver = lower('{address}'))
            AND lower(currency) = lower('{currency}')
            AND transfer_type = 'OSMOSIS'
        ),
        stakingRewards AS (
          SELECT *
          FROM osmosis.core.fact_staking_rewards
          WHERE delegator_address = lower('{address}') 
            AND lower(currency) = lower('{currency}')
        ),
        staking AS (
          SELECT *
          FROM osmosis.core.fact_staking
          WHERE delegator_address = lower('{address}') 
            AND lower(currency) = lower('{currency}')
        )

        SELECT 
          (SELECT decimal FROM osmosis.core.dim_tokens WHERE lower(address) = lower('{currency}')) AS decimal,
          (SELECT COALESCE(sum(from_amount), 0) FROM swaps WHERE lower(from_currency) = lower('{currency}')) AS netSwapOut,
          (SELECT COALESCE(sum(to_amount), 0) FROM swaps WHERE lower(to_currency) = lower('{currency}')) AS netSwapIn,
          (SELECT COALESCE(sum(amount), 0) FROM lpActions WHERE action = 'pool_joined') AS netLPDeposit,
          (SELECT COALESCE(sum(amount), 0) FROM lpActions WHERE action = 'pool_exited') AS netLPExit,
          (SELECT COALESCE(sum(amount), 0) FROM ibcTransfers WHERE transfer_type = 'IBC_TRANSFER_IN') AS netIBCIn,
          (SELECT COALESCE(sum(amount), 0) FROM ibcTransfers WHERE transfer_type = 'IBC_TRANSFER_OUT') AS netIBCOut,
          (SELECT COALESCE(sum(amount), 0) FROM transfers WHERE receiver = lower('{address}')) AS netTransferIn,
          (SELECT COALESCE(sum(amount), 0) FROM transfers WHERE sender = lower('{address}')) AS netTransferOut,
          (SELECT COALESCE(sum(amount), 0) FROM stakingRewards) AS netStakingRewards,
          (SELECT COALESCE(sum(amount), 0) FROM staking WHERE action = 'delegate') AS netStaked,
          (SELECT COALESCE(sum(amount), 0) FROM staking WHERE action = 'undelegate') AS netUnstaked
        """;

        var results = await Flipside.GetOrRunAsync<OsmosisFlowSankeyDTO>(sql, cancellationToken: cancellationToken);
        return results.First();
    }

    public async Task<string[]> GetDeveloperWalletsRecursiveAsync(int depth, CancellationToken cancellationToken)
    {
        var sqlBuilder = new StringBuilder(
        $"""
        WITH l0 AS (
          SELECT address
          FROM crosschain.core.address_tags
          WHERE tag_name = 'Developer Vesting Receiver' 
            AND tag_type = 'chadmin' 
            AND blockchain = 'osmosis'
        )
        """);

        for (int i = 1; i <= depth; i++)
        {
            sqlBuilder.Append(
            $"""
            ,
            l{i} AS (
              SELECT receiver AS address
              FROM osmosis.core.fact_transfers
              WHERE receiver NOT IN (SELECT address FROM l{i - 1})
                AND currency = 'uosmo' 
                AND transfer_type = 'OSMOSIS'
                AND amount >= POW(10, 5)
              GROUP BY receiver
              HAVING sum(CASE WHEN sender IN (SELECT address FROM l{i - 1}) THEN amount ELSE 0 END) >
                     sum(CASE WHEN sender NOT IN (SELECT address FROM l{i - 1}) THEN amount ELSE 0 END)
              UNION 
              SELECT *
              FROM l{i - 1}
            )
            """);
        }

        sqlBuilder.Append(
        $"""
        SELECT address FROM l{depth}
        """);

        string sql = sqlBuilder.ToString();

        var results = await Flipside.GetOrRunAsync<FlipsidePrimitiveObject<string>>(sql, cancellationToken: cancellationToken);
        return results.Select(x => x.Value).ToArray();
    }

    public async Task<string[]> GetRelatedAddressesAsync(string[] addresses, CancellationToken cancellationToken)
    {
        if (addresses.Length == 0)
        {
            return Array.Empty<string>();
        }

        string sql = FlipsideQueries.RelatedAddresses(addresses);
        var results = await Flipside.GetOrRunAsync<FlipsidePrimitiveObject<string>>(sql, cancellationToken: cancellationToken);
        return results.Select(x => x.Value).ToArray();
    }

    public async Task<OsmosisWalletRankingDTO> GetOsmosisWalletRankingAsync(string address, CancellationToken cancellationToken)
    {
        string sql = FlipsideQueries.OsmosisWalletRanking(address);
        var results = await Flipside.GetOrRunAsync<OsmosisWalletRankingDTO>(sql, cancellationToken: cancellationToken);
        var result = results.Single();

        // Workaround for inaccurate Balances
        result.BalanceAmount = await OsmosisLCD.GetCurrentOSMOBalanceAsync(address, cancellationToken);

        return result;
    }

    public async Task<OsmosisPoolInfoDTO[]> GetOsmosisPoolInfosAsync(uint[] poolIds, CancellationToken cancellationToken)
    {
        string sql = FlipsideQueries.OsmosisPoolInfos(poolIds);
        return await Flipside.GetOrRunAsync<OsmosisPoolInfoDTO>(sql, cancellationToken: cancellationToken);
    }
}
