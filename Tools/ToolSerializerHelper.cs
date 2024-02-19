using System.Windows.Media;
using System.Text.Json;

namespace Tools
{
    public static class ToolSerializerHelper
    {
        public static (Color color, ToolId? toolId) Read(ref Utf8JsonReader     reader
                                                       , JsonSerializerOptions? options)
        {
            ToolId? toolId = null;
            Color   color  = default;
            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    continue;
                }

                string? propertyName = reader.GetString();
                switch (propertyName)
                {
                    case "ToolId":
                        reader.Read();
                        toolId = JsonSerializer.Deserialize<ToolId>(ref reader, options);
                        break;
                    case "Color":
                    {
                        color = JsonSerializer.Deserialize<Color>(ref reader, options);
                        break;
                    }
                }
            }

            return (color, toolId);
        }

        public static void Write(Utf8JsonWriter        writer
                               , ITool                 value
                               , JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("ToolId");
            JsonSerializer.Serialize(writer, value.ToolId, options);

            writer.WritePropertyName("Color");
            JsonSerializer.Serialize(writer, value.Color, options);

            writer.WriteEndObject();
        }
    }
}