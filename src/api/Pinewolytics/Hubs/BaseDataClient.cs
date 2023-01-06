﻿using Common.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Pinewolytics.Hubs;

[InitializationPriority(-1)]
public abstract class BaseDataClient<THub, TReceiver> : Singleton, IDataClient
    where THub : Hub
    where TReceiver : class
{
    class RealtimeProperty
    {
        public string Name { get; }
        public TimeSpan Interval { get; }

        public object Value { get; set; } = null!; //null till InitializeAsync completes
        private Func<Task> RefreshInner { get; }

        public RealtimeProperty(string name, TimeSpan interval, Func<Task> refreshInner)
        {
            Name = name;
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

    [Inject]
    private readonly IHubContext<THub> HubContext = null!;

    private readonly RealtimeProperty[] Properties;

    public BaseDataClient()
    {
        string[] allowedNames = typeof(TReceiver).GetMethods().Select(x => x.Name).ToArray();

        Properties = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(x => x.GetCustomAttribute<RealtimeValue>() is not null)
            .Select(x =>
            {
                if (x.ReturnType.BaseType != typeof(Task))
                {
                    throw new InvalidOperationException($"RealTimeProperty {x.Name} must return a Task<T>");
                }

                var attribute = x.GetCustomAttribute<RealtimeValue>()!;

                if (!allowedNames.Contains(attribute.Name))
                {
                    throw new InvalidOperationException($"RealTimeProperty Name must be part of interface {typeof(TReceiver).Name}");
                }
                //
                return new RealtimeProperty(
                  attribute.Name,
                  TimeSpan.FromMilliseconds(attribute.MillisecondInterval),
                  () => (Task)x.Invoke(this, null)!);
            })
            .ToArray();
    }

    protected override async ValueTask InitializeAsync()
    {
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

                await SendPropertyToAsync(property, HubContext.Clients.All);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "There was an exception trying to refresh data");
            }
        }
    }

    public async Task SendWelcomeToAsync(string connectionId)
    {
        foreach(var property in Properties)
        {
            await SendPropertyToAsync(property, HubContext.Clients.Client(connectionId));
        }
    }

    private async Task SendPropertyToAsync(RealtimeProperty property, IClientProxy target)
    {
        if (property.Value is ITuple t)
        {
            await (t.Length switch
            {
                1 => target.SendAsync(property.Name, t[0]),
                2 => target.SendAsync(property.Name, t[0], t[1]),
                3 => target.SendAsync(property.Name, t[0], t[1], t[2]),
                _ => throw new InvalidOperationException("Tuple length not supported")
            });
        }
        else
        {
            await target.SendAsync(property.Name, property.Value);
        }
    }
}
