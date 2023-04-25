using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinewolytics.Utils;

public class BigIntegerNumberConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? raw = reader.TokenType == JsonTokenType.String
            ? reader.GetString()
            : JsonDocument.ParseValue(ref reader).RootElement.GetRawText();

        return raw is not null && BigInteger.TryParse(raw, NumberStyles.Float, null, out var result)
        ? result
        : throw new JsonException("Cant convert null to BigInteger");
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToString());
    }
}