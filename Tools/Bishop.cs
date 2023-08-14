using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Tools
{
    [JsonConverter(typeof(BishopConverter))]
    public class Bishop : ITool
    {
        public Color Color { get; }

        public string Type => "Bishop";

        public Bishop(Color color)
        {
            Color = color;
        }

        public ITool GetCopy()
        {
            return new Bishop(Color);
        }
    }

    public class BishopConverter : JsonConverter<Bishop>
    {
        public override Bishop? Read(ref Utf8JsonReader    reader
                                   , Type                  typeToConvert
                                   , JsonSerializerOptions options)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.PropertyName) { }

            string propertyName = reader.GetString();
            if (propertyName != "Color")
            {
                throw new SerializationException("Color property did not serialized");
            }

            reader.Read();
            reader.Read();
            byte A = (byte)reader.GetInt32();
            reader.Read();
            byte B = reader.GetByte();
            reader.Read();
            byte G = reader.GetByte();
            reader.Read();
            byte R = reader.GetByte();

            reader.Read(); // End array
            reader.Read(); // End object
            return new Bishop(Color.FromArgb(A, R, G, B));
        }

        public override void Write(Utf8JsonWriter        writer
                                 , Bishop                value
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
