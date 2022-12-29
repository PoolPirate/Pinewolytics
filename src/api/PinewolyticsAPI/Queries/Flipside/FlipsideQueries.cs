namespace Pinewolytics.Queries.Flipside;

public static class FlipsideQueries
{
    public static string ExternalNetOSMOTransfers(string[] addressCluster)
        => $"""
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

    public static string InternalNetOSMOTransfers(string[] addressCluster)
    => $"""
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

    public static string RelatedAddresses(string[] addressCluster)
        => $"""
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
