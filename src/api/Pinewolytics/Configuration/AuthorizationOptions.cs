using System.ComponentModel.DataAnnotations;

namespace Pinewolytics.Configuration;

public class AuthorizationOptions
{
    [Required]
    public string HangfireCookieSecret { get; set; } = null!;
}
