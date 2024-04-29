using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vulnerabilities.Api.Helpers;
public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        _ = DateTime.TryParse(reader.GetString(), out DateTime result);
        return result;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffK"));
    }
}