using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Pinewolytics.Models.DTOs;

public partial class DenominatedAmountDTO
{
    [GeneratedRegex("\\d+", RegexOptions.Compiled)]
    private static partial Regex LeadingNumberRegex();

    public required string Denom { get; init; }
    public required decimal Amount { get; init; }

    public static DenominatedAmountDTO FromBase64AttributeValue(string rawAttributeValue)
    {
        string attributeValue = Encoding.UTF8.GetString(Convert.FromBase64String(rawAttributeValue));

        var rawAmount = LeadingNumberRegex().Match(attributeValue).Value;

        return new DenominatedAmountDTO()
        {
            Amount = decimal.Parse(rawAmount, NumberStyles.Float),
            Denom = attributeValue[rawAmount.Length..],
        };
    }
}
