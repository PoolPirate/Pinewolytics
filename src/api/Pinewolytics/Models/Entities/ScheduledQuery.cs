namespace Pinewolytics.Models.Entities;

public class ScheduledQuery
{
    public string Name { get; private set; }
    public string Query { get; private set; }
    public string TypeName { get; private set; }
    public TimeSpan Interval { get; private set; }

    public ScheduledQuery(string name, string query, string typeName, TimeSpan interval)
    {
        Name = name;
        Query = query;
        TypeName = typeName;
        Interval = interval;
    }
}
