using System.Globalization;
using System.Numerics;

namespace Pinewolytics.Models.DTOs.All;

public class TimeSeriesEntryDTO<T> : IFlipsideObject<TimeSeriesEntryDTO<T>>
    where T : INumber<T>
{
    public required DateTimeOffset Timestamp { get; init; }
    public required T Value { get; init; }

    public static TimeSeriesEntryDTO<T> Parse(string[] rawValues)
    {
        return new TimeSeriesEntryDTO<T>()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            Value = T.Parse(rawValues[1], NumberStyles.Float, null)
        };
    }
}

public class TimeSeriesEntryDTO<T1, T2> : IFlipsideObject<TimeSeriesEntryDTO<T1, T2>>
    where T1 : INumber<T1>
    where T2 : INumber<T2>
{
    public required DateTimeOffset Timestamp { get; init; }
    public required T1 Value1 { get; init; }
    public required T2 Value2 { get; init; }

    public static TimeSeriesEntryDTO<T1, T2> Parse(string[] rawValues)
    {
        return new TimeSeriesEntryDTO<T1, T2>()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            Value1 = T1.Parse(rawValues[1], NumberStyles.Float, null),
            Value2 = T2.Parse(rawValues[2], NumberStyles.Float, null)
        };
    }
}

public class TimeSeriesEntryDTO<T1, T2, T3> : IFlipsideObject<TimeSeriesEntryDTO<T1, T2, T3>>
    where T1 : INumber<T1>
    where T2 : INumber<T2>
     where T3 : INumber<T3>
{
    public required DateTimeOffset Timestamp { get; init; }
    public required T1 Value1 { get; init; }
    public required T2 Value2 { get; init; }
    public required T3 Value3 { get; init; }
    public static TimeSeriesEntryDTO<T1, T2, T3> Parse(string[] rawValues)
    {
        return new TimeSeriesEntryDTO<T1, T2, T3>()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            Value1 = T1.Parse(rawValues[1], NumberStyles.Float, null),
            Value2 = T2.Parse(rawValues[2], NumberStyles.Float, null),
            Value3 = T3.Parse(rawValues[3], NumberStyles.Float, null)
        };
    }
}