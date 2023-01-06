using Common.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Pinewolytics.Configuration;

public class InstanceOptions : Option
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public bool RequireFullSync { get; set; } = true;
}
