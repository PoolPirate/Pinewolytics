using Hangfire.Annotations;

namespace Pinewolytics.Queries.Flipside;

public static class FlipsideQueries
{
    public static string ExternalNetOSMOTransfers(string[] addressCluster)
    {
        return $"""
        WITH scope AS (
        {string.Join(" UNION\n", addressCluster.Select(address => $"SELECT '{address}' AS address"))}
        ),
        transfers AS (
           SELECT *
           FROM osmosis.core.fact_transfers
           WHERE (lower(sender) IN (SELECT lower(address) FROM scope) OR lower(receiver) IN (SELECT lower(address) FROM scope))
             AND (lower(sender) NOT IN (SELECT lower(address) FROM scope) OR lower(receiver) NOT IN (SELECT lower(address) FROM scope))
             AND currency = 'uosmo' AND transfer_type = 'OSMOSIS'
             AND amount >= POW(10, 5)
        )

        SELECT sum(amount) / POW(10, 6) AS amount,
               sender, receiver 
        FROM transfers
        GROUP BY sender, receiver 
        """;
    }

    public static string InternalNetOSMOTransfers(string[] addressCluster)
    {
        return $"""
        WITH scope AS (
        {string.Join(" UNION\n", addressCluster.Select(address => $"SELECT '{address}' AS address"))}
        ),
        transfers AS (
           SELECT *
           FROM osmosis.core.fact_transfers
           WHERE lower(sender) IN (SELECT lower(address) FROM scope) AND lower(receiver) IN (SELECT lower(address) FROM scope)
             AND currency = 'uosmo' AND transfer_type = 'OSMOSIS'
             AND amount >= POW(10, 5)
        )

        SELECT sum(amount / POW(10, 6)) AS amount,
               sender, receiver 
        FROM transfers
        GROUP BY sender, receiver 
        """;
    }

    public static string RelatedAddresses(string[] addressCluster)
    {
        return $"""
        WITH scope AS (
        {string.Join(" UNION\n", addressCluster.Select(address => $"SELECT '{address}' AS address"))}
        )

        SELECT receiver AS address
        FROM osmosis.core.fact_transfers
        WHERE receiver NOT IN (SELECT address FROM scope)
        AND currency = 'uosmo' 
        AND transfer_type = 'OSMOSIS'
        AND amount >= POW(10, 5)
        GROUP BY receiver
        HAVING sum(CASE WHEN sender IN (SELECT address FROM scope) THEN amount ELSE 0 END) >
               sum(CASE WHEN sender NOT IN (SELECT address FROM scope) THEN amount ELSE 0 END)
        
        """;
    }

    public static string OsmosisWalletRanking(string address)
    {
        return $"""
            WITH staked AS (
              SELECT delegator_address AS address,
                sum(CASE WHEN action = 'delegate' THEN amount WHEN action = 'undelegate' THEN -amount ELSE 0 END) 
                / POW(10, 6) AS net_staked,
                ROW_NUMBER() OVER (ORDER BY net_staked DESC) 
                AS rank
              FROM osmosis.core.fact_staking
              GROUP BY delegator_address
              HAVING net_staked > 0
            ),
            lpd_0 AS (
              SELECT liquidity_provider_address AS address, pool_id[0] AS pid,
                sum(CASE WHEN action = 'lp_tokens_minted' THEN amount WHEN action = 'lp_tokens_burned' THEN -amount ELSE 0 END)
                AS net_deposited,
                ROW_NUMBER() OVER (PARTITION BY pid ORDER BY net_deposited DESC) 
                AS rank
              FROM osmosis.core.fact_liquidity_provider_actions
              GROUP BY liquidity_provider_address, pid
              HAVING net_deposited > 0
            ),
            lpd AS (
              SELECT address, 
                array_agg(pid) WITHIN GROUP (ORDER BY pid) AS pids,
                array_agg(net_deposited) WITHIN GROUP (ORDER BY pid) AS net_deposits,
                array_agg(rank) WITHIN GROUP (ORDER BY pid) AS ranks
              FROM lpd_0
              GROUP BY address
            ),
            scope AS (
              SELECT '{address}' AS address
            )

            SELECT sc.address,   
              (SELECT max(block_timestamp) FROM osmosis.core.fact_blocks) AS last_updated_at,
              COALESCE(st.net_staked, 0) AS staked, COALESCE(st.rank, -1) AS staked_rank,
              COALESCE(lp.pids, []), COALESCE(lp.net_deposits, []), COALESCE(lp.ranks, [])
            FROM scope sc
            LEFT JOIN staked st ON sc.address = st.address
            LEFT JOIN lpd lp ON sc.address = lp.address
            """;
    }
}
