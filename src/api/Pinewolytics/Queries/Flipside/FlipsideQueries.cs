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

    public static string OsmosisPoolInfos(uint[] poolIds)
    {
        return $"""
            WITH scope AS (
            {string.Join(" UNION\n", poolIds.Select(pid => $"SELECT '{pid}' AS pid"))}
            ),  
            poolassets AS (
              SELECT p.pool_id, f1.value:asset_address AS asset
              FROM osmosis.core.dim_liquidity_pools p,
              LATERAL flatten(input => assets) f1
            ),
            pools AS (
              SELECT pool_id, ARRAY_AGG(COALESCE(project_name, asset)) AS assets
              FROM poolassets
              LEFT JOIN osmosis.core.dim_tokens ON asset = address
              GROUP BY pool_id
            )

            SELECT pid, assets
            FROM scope
            JOIN pools ON pid = pool_id
            
            """;
    }

    public static string ListICNSNames()
    {
        return $"""
            SELECT tag_name, address
            FROM crosschain.core.address_tags
            WHERE tag_name ILIKE '%.osmo' AND tag_type = 'ICNS'
            QUALIFY 1 = ROW_NUMBER() OVER (PARTITION BY tag_name ORDER BY tag_created_at DESC)
            """;
    }

    public static string ListProtoRevTransactions()
    {
        return $"""
            WITH arb_tx AS (
              SELECT DISTINCT tx_id, block_timestamp
              FROM osmosis.core.fact_msg_attributes
              WHERE msg_type = 'coinbase'
                AND attribute_key = 'minter'
                AND attribute_value = 'osmo17qdmjdumw4xawam4g46gtwzle5rd4zwyfqvvza'
                AND block_timestamp >= CURRENT_DATE - 180
            ),
            receivals AS (
              SELECT a1.tx_id, 
                SUBSTR(a2.attribute_value, LENGTH(REGEXP_SUBSTR(a2.attribute_value, '\\d+')) + 1) AS currency,
                REGEXP_SUBSTR(a2.attribute_value, '\\d+')::numeric AS amount
              FROM osmosis.core.fact_msg_attributes a1, osmosis.core.fact_msg_attributes a2
              WHERE a1.msg_type = 'coin_received' AND a1.msg_type = a2.msg_type
                AND a1.tx_id = a2.tx_id
                AND a1.msg_index = a2.msg_index
                AND a1.attribute_key = 'receiver' AND a2.attribute_key = 'amount'
                AND a1.attribute_value = 'osmo17qdmjdumw4xawam4g46gtwzle5rd4zwyfqvvza'
            ),
            total_rec AS (
              SELECT tx_id, r.currency,
                sum(amount) AS total_received
              FROM receivals r
              GROUP BY tx_id, r.currency
            ),
            sends AS (
              SELECT a1.tx_id, 
                SUBSTR(a2.attribute_value, LENGTH(REGEXP_SUBSTR(a2.attribute_value, '\\d+')) + 1) AS currency,
                REGEXP_SUBSTR(a2.attribute_value, '\\d+')::numeric AS amount
              FROM osmosis.core.fact_msg_attributes a1, osmosis.core.fact_msg_attributes a2
              WHERE a1.msg_type = 'coin_spent' AND a1.msg_type = a2.msg_type
                AND a1.tx_id = a2.tx_id
                AND a1.msg_index = a2.msg_index
                AND a1.attribute_key = 'spender' AND a2.attribute_key = 'amount'
                AND a1.attribute_value = 'osmo17qdmjdumw4xawam4g46gtwzle5rd4zwyfqvvza'
            ),
            total_send AS (
              SELECT tx_id, s.currency,
                sum(amount) AS total_sent
              FROM sends s
              GROUP BY tx_id, s.currency
            ),
            arb_basis AS (
              SELECT DISTINCT t.tx_id, t.block_timestamp, currency
              FROM arb_tx t
              JOIN total_rec r ON t.tx_id = r.tx_id
              UNION
              SELECT DISTINCT t.tx_id, t.block_timestamp, currency
              FROM arb_tx t
              JOIN total_send s ON t.tx_id = s.tx_id
            ),
            arbs AS (
              SELECT t.tx_id, t.block_timestamp, t.currency,
                NVL(sum(total_received), 0) 
                - NVL(sum(total_sent), 0) AS arbitrage_amount,
                NVL(sum(total_received * price / POW(10, decimal)), 0) 
                - NVL(sum(total_sent * price / POW(10, decimal)), 0) AS arbitrage_usd
              FROM arb_basis t
              LEFT JOIN total_rec r ON r.tx_id = t.tx_id AND r.currency = t.currency
              LEFT JOIN total_send s ON s.tx_id = t.tx_id AND s.currency = t.currency
              LEFT JOIN osmosis.core.ez_prices p 
                ON DATE_TRUNC('hour', t.block_timestamp) = recorded_hour 
                AND t.currency = p.currency 
              JOIN osmosis.core.dim_tokens ON address = t.currency
              GROUP BY t.tx_id, t.block_timestamp, t.currency
              HAVING arbitrage_amount > 0
            ),
            biggest_tx AS (
              SELECT tx_id
              FROM arbs
              GROUP BY tx_id
              ORDER BY sum(arbitrage_usd) DESC
            )

            SELECT tx.tx_id, tr.block_timestamp, tx_from,
              ARRAY_AGG(currency) AS currencies,
              ARRAY_AGG(arbitrage_amount) AS arb_amounts,
              ARRAY_AGG(arbitrage_usd) AS arb_amounts_usd
            FROM biggest_tx tx
            JOIN arbs arb ON tx.tx_id = arb.tx_id
            JOIN osmosis.core.fact_transactions tr ON tr.tx_id = tx.tx_id 
            GROUP BY tx.tx_id, tr.block_timestamp, tx_from
            """;
    }

    public static string ListWalletRanking()
    {
        return """
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
              SELECT tx_from AS address
              FROM osmosis.core.fact_transactions
              WHERE tx_from IS NOT NULL
              GROUP BY tx_from
              ORDER BY count(*) DESC
              LIMIT 500000
            ),
            latestbalances AS (
              SELECT address, max(balance) / POW(10, 6) AS net_balance,
                ROW_NUMBER() OVER (ORDER BY net_balance DESC) AS rank
              FROM osmosis.core.fact_daily_balances
              WHERE currency = 'uosmo' 
                AND balance_type = 'liquid' 
                AND date = (SELECT max(date) FROM osmosis.core.fact_daily_balances)
              GROUP BY address
            )

            SELECT sc.address,   
              (SELECT max(block_timestamp) FROM osmosis.core.fact_blocks) AS last_updated_at,
              COALESCE(st.net_staked, 0) AS staked, COALESCE(st.rank, -1) AS staked_rank,
              COALESCE(bl.net_balance, 0) AS balance, COALESCE(bl.rank, -1) AS balance_rank,
              COALESCE(lp.pids, []), COALESCE(lp.net_deposits, []), COALESCE(lp.ranks, [])
            FROM scope sc
            LEFT JOIN staked st ON sc.address = st.address
            LEFT JOIN latestbalances bl ON sc.address = bl.address
            LEFT JOIN lpd lp ON sc.address = lp.address
            """;
    }
}
