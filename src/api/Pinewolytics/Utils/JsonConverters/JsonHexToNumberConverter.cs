using Newtonsoft.Json;
using System.Globalization;
using System.Numerics;

namespace Pinewolytics.Utils.JsonConverters;

public class JsonHexToNumberConverter<T> : JsonConverter<T>
    where T : INumber<T>
{
    public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return T.Parse(reader.ReadAsString(), NumberStyles.HexNumber, null);
    }

    public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
