using Hangfire.Dashboard;
using Pinewolytics.Configuration;

namespace Pinewolytics.Utils;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private const string HangfireCookieName = "Hangfire";

    public static bool IsReadOnlyAccess(DashboardContext context, AuthorizationOptions options, bool isProduction)
    {
        if (!context.GetHttpContext().Request.Cookies.ContainsKey(HangfireCookieName)) {
            return true;
        }

        var hangfireCookie = context.GetHttpContext().Request.Cookies.Single(x => x.Key == HangfireCookieName);
        return hangfireCookie.Value != options.HangfireCookieSecret;
    }

    public bool Authorize(DashboardContext context)
        => true;
}