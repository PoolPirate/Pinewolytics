using Pinewolytics.Models.FlipsideAPI.Requests;

namespace Pinewolytics.Models.FlipsideAPI;

public class FlipsideRequest
{
    public string Jsonrpc { get; private set; } = "2.0";
    public int Id { get; private set; } = 1;
    public required string Method { get; init; }
    public required object[] Params { get; init; }

    public static FlipsideRequest CreateQueryRun(int resultTTLHours, int maxAgeMinutes, string sql) 
        => new FlipsideRequest()
        {
            Method = "createQueryRun",
            Params = new IFlipsideRequestParams[]
                {
                    new CreateQueryRunParams(resultTTLHours, maxAgeMinutes, sql, "snowflake-default", "flipside")
                }
        };

    public static FlipsideRequest GetQueryRun(string queryRunId) 
        => new FlipsideRequest()
        {
            Method = "getQueryRun",
            Params = new IFlipsideRequestParams[]
                {
                    new GetQueryRunParams(queryRunId)
                }
        };

    public static FlipsideRequest GetQueryRunResults(string queryRunId, int page, int pageSize)
        => new FlipsideRequest()
        {
            Method = "getQueryRunResults",
            Params = new IFlipsideRequestParams[]
                {
                    new GetQueryRunResultsParams(queryRunId, "csv", page, pageSize)
                }
        };

    public static FlipsideRequest CancelQueryRun(string queryRunId)
        => new FlipsideRequest()
        {
            Method = "cancelQueryRun",
            Params = new IFlipsideRequestParams[]
                {
                    new CancelQueryRunParams(queryRunId)
                }
        };
}
