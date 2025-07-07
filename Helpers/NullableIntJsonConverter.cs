using System.Text.Json;
using System.Text.Json.Serialization;

public class NullableIntJsonConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str)) return null;

            if (int.TryParse(str, out var val)) return val;
            throw new JsonException("Invalid integer format.");
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var num))
        {
            return num;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}

