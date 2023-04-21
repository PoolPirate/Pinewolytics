using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinewolytics.Utils;

public class BigIntegerNumberConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? raw = reader.GetString();

        return raw switch
        {
            null => throw new JsonException("Cant convert null to BigInteger"),
            _ => BigInteger.Parse(raw)
        };
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToString());
    }
}