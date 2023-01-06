using Common.Services;
using Pinewolytics.Configuration;

namespace Pinewolytics.Services.ApiClients;

public class OptimismRpcClient : EVMRpcClient
{
    [Inject]
    private readonly ApiKeyOptions ApiKeyOptions = null!;

    protected override Uri GetAPIUrl() 
        => new Uri(ApiKeyOptions.OptimismRPCProviderUrl, UriKind.Absolute);
}
