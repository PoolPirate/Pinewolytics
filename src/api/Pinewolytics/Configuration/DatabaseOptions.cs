using Common.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Pinewolytics.Configuration;

public class DatabaseOptions : Option
{
    [Required]
    public string AppConnectionString { get; set; } = null!;
    [Required]
    public string HangfireConnectionString { get; set; } = null!;

    [Required]
    public string RedisConnectionString { get; set; } = null!;
}
