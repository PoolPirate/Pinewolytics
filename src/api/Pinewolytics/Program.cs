using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Configuration;
using Pinewolytics.Database;
using System.Reflection;

namespace Pinewolytics;

public class Program
{
    public const int ShutdownTimeout = 5000;

    public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

    public static Startup Startup { get; private set; } = null!;
    public static WebApplication Application { get; private set; } = null!;
    public static IServiceProvider Provider { get; private set; } = null!;
    public static ILogger Logger { get; private set; } = null!;

    public static async Task Main(string[] args)
    {
        Application = CreateApplication(args);
        Provider = Application.Services;
        Logger = Application.Services.GetRequiredService<ILogger<Startup>>();

        await MigrateDatabseAsync();

        await Provider.InitializeApplicationAsync(Assembly);

        Provider.RunApplication(Assembly);

        string url = GetListenUrl();
        Application.Run(url);
    }

    static WebApplication CreateApplication(string[] args)
    {
        var webApp = WebApplication.CreateBuilder(args);

        webApp.Logging.AddConsole();
        webApp.Configuration.AddJsonFile("appsettings.json", false);

        Startup = new Startup(webApp.Configuration);
        Startup.ConfigureServices(webApp.Services);

        var host = webApp.Build();
        Startup.ConfigurePipeline(host, webApp.Environment);
        Startup.ConfigureRoutes(host);

        return host;
    }

    private static string GetListenUrl()
    {
        var bindingOptions = Provider.GetRequiredService<BindingOptions>();
        return $"http://{bindingOptions.BindAddress}:{bindingOptions.ApplicationPort}";
    }

    private static async Task MigrateDatabseAsync()
    {
        using var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PinewolyticsContext>();

        await dbContext.Database.MigrateAsync();
    }

    public static async Task ShutdownAsync()
    {
        var lifetime = Provider!.GetRequiredService<IHostLifetime>();
        using var cts = new CancellationTokenSource(ShutdownTimeout);
        await lifetime.StopAsync(cts.Token);
        Environment.Exit(0);
    }
}