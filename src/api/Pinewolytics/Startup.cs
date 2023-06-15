using Common.Extensions;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Configuration;
using Pinewolytics.Database;
using Pinewolytics.Hubs;
using Pinewolytics.Services;
using Pinewolytics.Utils;
using Polly;
using Polly.Contrib.WaitAndRetry;
using StackExchange.Redis;
using System.Net;

namespace Pinewolytics;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new BigIntegerNumberConverter());
        });

        services.AddHttpClient<FlipsideClient>()
            .AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder.Or<HttpRequestException>(ex => ex.StatusCode == HttpStatusCode.BadGateway)
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(200), 5)));

        services.AddApplication(Configuration, options =>
        {
            options.UseServiceLevels = false;
            options.ValidateServiceLevelsOnInitialize = true;
            options.IgnoreIServiceWithoutLifetime = false;
        },
        Program.Assembly);

        services.AddControllers();

        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var databaseOptions = provider.GetRequiredService<DatabaseOptions>();
            return ConnectionMultiplexer.Connect(databaseOptions.RedisConnectionString);
        });

        services.AddResponseCaching();

        services.AddDbContext<PinewolyticsContext>((provider, options) =>
        {
            var dbOptions = provider.GetRequiredService<DatabaseOptions>();
            options.UseNpgsql(dbOptions.AppConnectionString);
        });

        services.AddHangfire((provider, config) =>
        {
            var dbOptions = provider.GetRequiredService<DatabaseOptions>();
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseActivator(new ServiceProviderJobActivator(provider))
                  .UsePostgreSqlStorage(dbOptions.HangfireConnectionString, new PostgreSqlStorageOptions()
                  {
                      PrepareSchemaIfNecessary = true
                  });
        });

        services.AddSignalR();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void ConfigurePipeline(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors(policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
        });

        app.UseStaticFiles();

        var options = new RewriteOptions()
            .AddRewrite("^(?!api\\/|Api\\/)(.+)\\/$", "$1.html", true)
            .AddRewrite("^(?!api\\/|Api\\/)(.+)", "$1.html", true);

        app.UseRewriter(options);

        app.UseStaticFiles();

        app.UseHangfireDashboard("/api/hangfire", new DashboardOptions()
        {
            Authorization = new[] { new HangfireDashboardAuthorizationFilter() },
            IsReadOnlyFunc =
                ctx => HangfireDashboardAuthorizationFilter.IsReadOnlyAccess(
                    ctx, app.ApplicationServices.GetRequiredService<AuthorizationOptions>(), env.IsProduction())
        });

        app.UseResponseCaching();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
    }

    public void ConfigureRoutes(IEndpointRouteBuilder routes)
    {
        routes.MapHub<SubscriptionHub>("api/hub/query");

        routes.MapControllers();

        routes.MapFallbackToFile("index.html");
    }
}