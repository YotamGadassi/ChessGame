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
                        reader.Read();
                        reader.Read();
                        byte A = (byte)reader.GetInt32();
                        reader.Read();
                        byte B = reader.GetByte();
                        reader.Read();
                        byte G = reader.GetByte();
                        reader.Read();
                        byte R = reader.GetByte();
                        color = Color.FromArgb(A, B, G, R);
                        reader.Read(); // End array
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

            writer.WriteStartArray("Color");
            writer.WriteNumberValue(value.Color.A);
            writer.WriteNumberValue(value.Color.B);
            writer.WriteNumberValue(value.Color.G);
            writer.WriteNumberValue(value.Color.R);
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}