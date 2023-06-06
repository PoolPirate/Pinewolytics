using System;

namespace Pinewolytics.Utils;

public static class CronUtils
{
    public static string ConvertFromPeriodRecurrence(TimeSpan periodRecurrence)
    {
        if (periodRecurrence.Seconds > 0 || periodRecurrence.Milliseconds > 0)
        {
            throw new ArgumentException("Interval cannot contain seconds nor milliseconds.");
        }
        if (periodRecurrence.Hours > 1 && (periodRecurrence.Minutes > 0 || periodRecurrence.Seconds > 0))
        {
            throw new ArgumentException("Intervals cannot contain minutes if they are larger than 1 hour");
        }
        if (periodRecurrence.Days > 1 && (periodRecurrence.Hours > 0 || periodRecurrence.Minutes > 0 || periodRecurrence.Seconds > 0))
        {
            throw new ArgumentException("Intervals cannot contain hours or minutes if they are larger than 24 hours");
        }

        if (periodRecurrence.Days >= 1)
        {
            return $"0 0 */{periodRecurrence.Days} * *";
        }
        if (periodRecurrence.TotalHours >= 1)
        {
            return $"0 */{periodRecurrence.Hours} * * *";
        }
        if (periodRecurrence.Minutes > 1)
        {
            return $"*/{periodRecurrence.Minutes} * * * *";
        }
        //
        return $"*/1 * * * *";
    }
}
