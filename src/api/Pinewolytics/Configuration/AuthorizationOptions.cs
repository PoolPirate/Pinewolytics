using Common.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Pinewolytics.Configuration;

public class AuthorizationOptions : Option
{
    [Required]
    public string HangfireCookieSecret { get; set; } = null!;
}
