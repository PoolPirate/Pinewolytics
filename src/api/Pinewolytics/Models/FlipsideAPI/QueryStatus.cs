namespace Pinewolytics.Models.FlipsideAPI;

public static class QueryStatus
{
    public const string Success = "QUERY_STATE_SUCCESS";

    public const string Cancelled = "QUERY_STATE_CANCELED";
    public const string Failed = "QUERY_STATE_FAILED";

    public const string Ready = "QUERY_STATE_READY";
    public const string Pending = "QUERY_STATE_STREAMING_RESULTS";
    public const string Running = "QUERY_STATE_RUNNING";
}
