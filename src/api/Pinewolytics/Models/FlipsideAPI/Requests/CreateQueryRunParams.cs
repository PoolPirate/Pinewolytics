namespace Pinewolytics.Models.FlipsideAPI.Requests;

public class CreateQueryRunParams : IFlipsideRequestParams
{
    public int ResultTTLHours { get; private set; } = 1;
    public int MaxAgeMinutes { get; private set; } = 1;
    public string Sql { get; private set; }

    public Tags Tags { get; private set; }

    public string DataSource { get; private set; }
    public string DataProvider { get; private set; }

    public CreateQueryRunParams(int resultTTLHours, int maxAgeMinutes, string sql, string dataSource, string dataProvider)
    {
        ResultTTLHours = resultTTLHours;
        MaxAgeMinutes = maxAgeMinutes;
        Sql = sql;
        DataSource = dataSource;
        DataProvider = dataProvider;

        Tags = new Tags("pinewolytics", "main");
    }
}

public class Tags
{
    public string Source { get; private set; }
    public string Env { get; private set; }

    public Tags(string source, string env)
    {
        Source = source;
        Env = env;
    }
}
