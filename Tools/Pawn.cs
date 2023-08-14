using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Tools
{
    [JsonConverter(typeof(PawnConverter))]
    public class Pawn : ITool
    {
        public string Type => "Pawn";
        [JsonConverter(typeof(ColorConverter))]
        public Color Color { get; }

        public Pawn(Color color)
        {
            Color = color;
        }

        public ITool GetCopy()
        {
            Pawn newPawn = new Pawn(Color);

            return newPawn;
        }

    }

    public class PawnConverter : JsonConverter<Pawn>
    {
        public override Pawn? Read(ref Utf8JsonReader    reader
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

            return new Pawn(Color.FromArgb(A, R, G, B));
        }

        public override void Write(Utf8JsonWriter        writer
                                 , Pawn                  value
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
