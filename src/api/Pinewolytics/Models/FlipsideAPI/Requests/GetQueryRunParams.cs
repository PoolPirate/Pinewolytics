namespace Pinewolytics.Models.FlipsideAPI.Requests;

public class GetQueryRunParams : IFlipsideRequestParams
{
    public string QueryRunId { get; private set; }

    public GetQueryRunParams(string queryRunId)
    {
        QueryRunId = queryRunId;
    }
}
