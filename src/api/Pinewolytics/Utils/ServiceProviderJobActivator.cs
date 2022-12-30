using Hangfire;

namespace Pinewolytics.Utils;

public class ServiceProviderJobActivator : JobActivator
{
    private readonly IServiceProvider Provider;

    public ServiceProviderJobActivator(IServiceProvider provider)
    {
        Provider = provider;
    }

    public override object ActivateJob(Type jobType)
    {
        return Provider.GetRequiredService(jobType);
    }
}
