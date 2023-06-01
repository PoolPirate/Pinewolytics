namespace Pinewolytics.Models.FlipsideAPI;

public class QueryRun
{
    public required string Id { get; init; }
    public required string State { get; init; }
    public required int? RowCount { get; init; }
}
