namespace Pinewolytics.Models.FlipsideAPI.Results;

public class GetQueryRunResult : IFlipsideRequestResult
{
    public required QueryRun QueryRun { get; init; }
}
