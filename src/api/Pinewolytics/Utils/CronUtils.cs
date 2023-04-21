namespace Pinewolytics.Utils;

public static class CronUtils
{
    public static string ConvertFromPeriodRecurrence(TimeSpan periodRecurrence)
    {
        if (periodRecurrence.Seconds > 0 || periodRecurrence.Milliseconds > 0)
        {
            throw new ArgumentException("Interval cannot contain seconds nor milliseconds.");
        }
        if (periodRecurrence.Hours >= 1)
        {
            return $"{periodRecurrence.Minutes} */{periodRecurrence.Hours} * * *";
        }
        if (periodRecurrence.Minutes > 1)
        {
            return $"*/{periodRecurrence.Minutes} * * * *";
        }
        //
        return $"*/1 * * * *";
    }
}
