namespace Pinewolytics.Models.FlipsideAPI.Results;

public class CreateQueryRunResult : IFlipsideRequestResult
{
    public required QueryRequest QueryRequest { get; init; }
    public required QueryRun QueryRun { get; init; }
}
