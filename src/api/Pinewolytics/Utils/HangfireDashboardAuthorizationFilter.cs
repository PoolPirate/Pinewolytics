using Hangfire.Dashboard;
using Pinewolytics.Configuration;

namespace Pinewolytics.Utils;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private const string HangfireCookieName = "Hangfire";

    public static bool AllowWriteAccess(DashboardContext context, AuthorizationOptions options, bool isProduction)
    {
        var hangfireCookie = context.GetHttpContext().Request.Cookies.SingleOrDefault(x => x.Key == HangfireCookieName);
        return !isProduction || hangfireCookie.Value == options.HangfireCookieSecret;
    }

    public bool Authorize(DashboardContext context)
        => true;
}