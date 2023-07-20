namespace Pinewolytics.Models.FlipsideAPI.Requests;

public class CancelQueryRunParams : IFlipsideRequestParams
{
    public string QueryRunId { get; private set; }

    public CancelQueryRunParams(string queryRunId)
    {
        QueryRunId = queryRunId;
    }
}
