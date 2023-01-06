using Common.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Pinewolytics.Configuration;

[SectionName("ApiKeys")]
public class ApiKeyOptions : Option
{
    [Required]
    public string FlipsideApiKey { get; set; } = null!;

    [Required]
    public string OptimismRPCProviderUrl { get; set; } = null!;
}
