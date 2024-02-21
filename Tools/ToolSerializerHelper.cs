using System.Runtime.Serialization;
using System.Windows.Media;
using System.Text.Json;

namespace Tools
{
    public static class ToolSerializerHelper
    {
        public static (Color color, ToolId? toolId) Read(ref Utf8JsonReader     reader
                                                       , JsonSerializerOptions? options)
        {
            Color  color  = default(Color);
            ToolId toolId = default(ToolId);

            if (reader.TokenType != JsonTokenType.StartObject)
                reader.Read(); // Start Object

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new SerializationException("Serialization Error for type: Tool");
            }

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    reader.Read();
                    continue;
                }

                string? propertyName = reader.GetString();
                switch (propertyName)
                {
                    case "ToolId":
                        reader.Read(); // Start Object
                        toolId = JsonSerializer.Deserialize<ToolId>(ref reader, options);
                        reader.Read(); // End Object
                        break;
                    case "Color":
                    {
                        reader.Read(); // Start Object
                        color = JsonSerializer.Deserialize<Color>(ref reader, options);
                        reader.Read(); // End Object
                        break;
                    }
                }
            }

            reader.Read();
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