namespace Pinewolytics.Models.FlipsideAPI.Results;

public class GetQueryRunResultsResult : IFlipsideRequestResult
{
    public required string[] ColumnNames { get; init; }
    public required string[] ColumnTypes { get; init; }

    public required object[][] Rows { get; init; }

    public required QueryRun OriginalQueryRun { get; init; }
}
