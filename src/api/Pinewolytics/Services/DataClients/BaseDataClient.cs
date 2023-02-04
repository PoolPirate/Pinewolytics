using Common.Services;
using Microsoft.AspNetCore.SignalR;
using Pinewolytics.Hubs;
using Pinewolytics.Services;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Pinewolytics.Services.DataClients;

[InitializationPriority(-1)]
public abstract class BaseDataClient : Singleton
{
    class RealtimeProperty
    {
        public string Key { get; }
        public TimeSpan Interval { get; }

        public object Value { get; set; } = null!; //null till InitializeAsync completes
        private Func<Task> RefreshInner { get; }

        public RealtimeProperty(string key, TimeSpan interval, Func<Task> refreshInner)
        {
            Key = key;
            Interval = interval;
            RefreshInner = refreshInner;
        }

        public async Task<bool> RefreshAsync()
        {
            var task = RefreshInner.Invoke();
            await task;

            var resultProperty = typeof(Task<>)
                .MakeGenericType(task.GetType().GenericTypeArguments[0])
                .GetProperty("Result")!;

            object result = resultProperty.GetValue(task)!;
            bool changed = !result.Equals(Value);
            Value = result;
            return changed;
        }
    }

    protected const int SECONDS = 1000;
    protected const int MINUTES = 60 * SECONDS;
    protected const int HOURS = 60 * MINUTES;

    private SocketSubscriptionService SocketSubscriptionService = null!;

    private readonly RealtimeProperty[] Properties;

    public BaseDataClient()
    {
        Properties = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(x => x.GetCustomAttribute<RealtimeValue>() is not null)
            .Select(x =>
            {
                if (x.ReturnType.BaseType != typeof(Task))
                {
                    throw new InvalidOperationException($"RealTimeProperty {x.Name} must return a Task<T>");
                }

                var attribute = x.GetCustomAttribute<RealtimeValue>()!;

                return new RealtimeProperty(
                        attribute.Key,
                        TimeSpan.FromMilliseconds(attribute.MillisecondInterval),
                        () => (Task)x.Invoke(this, null)!);
            })
            .ToArray();
    }

    protected override async ValueTask InitializeAsync()
    {
        SocketSubscriptionService = Provider.GetRequiredService<SocketSubscriptionService>();

        Logger.LogInformation("Initializing RealtimeValue methods");
        await Parallel.ForEachAsync(Properties, async (property, cancellationToken) => await property.RefreshAsync());
    }

    protected override ValueTask RunAsync()
    {
        foreach (var property in Properties)
        {
            _ = RunRealtimeValueLoopAsync(property);
        }

        return base.RunAsync();
    }

    private async Task RunRealtimeValueLoopAsync(RealtimeProperty property)
    {
        var timer = new PeriodicTimer(property.Interval);

        while (true)
        {
            await timer.WaitForNextTickAsync();

            try
            {
                if (!await property.RefreshAsync())
                {
                    continue;
                }

                await SocketSubscriptionService.BroadcastRealtimeValueUpdate(property.Key, property.Value);
            }
            catch (Exception ex)
            {
                Logger.LogWarning("There was an exception trying to refresh data for {name}", property.Key);
                Logger.LogDebug(ex, "Realtime Value Update Stacktrace");
            }
        }
    }

    public async Task<bool> SendPropertyToAsync(string key, ISubscriptionHubClient target)
    {
        var property = Properties.SingleOrDefault(x => x.Key == key);

        if (property is null)
        {
            return false;
        }

        await target.SendRealtimeValue(property.Key, property.Value);
        return true;
    }
}
