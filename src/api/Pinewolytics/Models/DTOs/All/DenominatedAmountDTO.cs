using Pinewolytics.Utils;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Pinewolytics.Models.DTOs;

public partial class DenominatedAmountDTO
{
    [GeneratedRegex("\\d+", RegexOptions.Compiled)]
    private static partial Regex LeadingNumberRegex();

    public required string Denom { get; init; }
    [JsonConverter(typeof(BigIntegerNumberConverter))]
    public required BigInteger Amount { get; init; }

    public static DenominatedAmountDTO FromBase64AttributeValue(string rawAttributeValue)
    {
        string attributeValue = Encoding.UTF8.GetString(Convert.FromBase64String(rawAttributeValue));

        string rawAmount = LeadingNumberRegex().Match(attributeValue).Value;

        return new DenominatedAmountDTO()
        {
            Amount = BigInteger.Parse(rawAmount),
            Denom = attributeValue[rawAmount.Length..],
        };
    }
}
