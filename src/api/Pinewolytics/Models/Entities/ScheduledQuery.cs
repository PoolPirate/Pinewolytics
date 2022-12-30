namespace Pinewolytics.Models.Entities;

public class ScheduledQuery
{
    public string Name { get; set; }
    public string Query { get; set; }
    public TimeSpan Interval { get; set; }

    public ScheduledQuery(string name, string query, TimeSpan interval)
    {
        Name = name;
        Query = query;
        Interval = interval;
    }
}
