using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;
using Common;

namespace Tools
{
    [JsonConverter(typeof(KnightConverter))]
    public class Knight : ITool
    {
        public string Type => "Knight";

        public Color Color { get; }

        public Knight(Color color)
        {
            Color = color;
        }

        public ITool GetCopy()
        {
            Knight newKnight = new Knight(Color);

            return newKnight;
        }

    }

    public class KnightConverter : JsonConverter<Knight>
    {
        public override Knight? Read(ref Utf8JsonReader    reader
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

            return new Knight(Color.FromArgb(A, R, G, B));
        }

        public override void Write(Utf8JsonWriter        writer
                                 , Knight                value
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
