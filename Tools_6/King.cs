using System.Text.Json.Serialization;
using System.Text.Json;
using System.Windows.Media;
using Common;

namespace Tools
{
    [JsonConverter(typeof(KingConverter))]
    public class King : ITool
    {
        public Color Color { get; }

        public string Type => "King";

        public King(Color color)
        {
            Color = color;
        }

        public ITool GetCopy()
        {
            return new King(Color);
        }
    }

    
    public class KingConverter : JsonConverter<King>
    {
        public override King? Read(ref Utf8JsonReader    reader
                                 , Type                  typeToConvert
                                 , JsonSerializerOptions options)
        {
            reader.Read();
            reader.Read();
            reader.Read();
            reader.Read();
            reader.Read();

            byte A = (byte)reader.GetInt32();
            reader.Read();
            byte B = reader.GetByte();
            reader.Read();
            byte G = reader.GetByte();
            reader.Read();
            byte R = reader.GetByte();
            reader.Read();
            reader.Read();
            reader.Read();

            reader.Read();

            return new King(Color.FromArgb(A, R, G, B));
        }

        public override void Write(Utf8JsonWriter        writer
                                 , King                  value
                                 , JsonSerializerOptions options)
        {
            writer.WriteStartObject();
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
