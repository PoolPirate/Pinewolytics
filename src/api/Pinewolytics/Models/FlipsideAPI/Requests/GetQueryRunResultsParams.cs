namespace Pinewolytics.Models.FlipsideAPI.Requests;

public class GetQueryRunResultsParams : IFlipsideRequestParams
{
    public string QueryRunId { get; set; }
    public string Format { get; set; }
    public PageParams Page { get; set; }

    public GetQueryRunResultsParams(string queryRunId, string format, int page, int pageSize)
    {
        QueryRunId = queryRunId;
        Format = format;
        Page = new PageParams(page, pageSize);
    }
}

public class PageParams
{
    public int Number { get; set; }
    public int Size { get; set; }

    public PageParams(int number, int size)
    {
        Number = number;
        Size = size;
    }
}
