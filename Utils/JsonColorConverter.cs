using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Utils;

public class JsonColorConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader    reader
                             , Type                  typeToConvert
                             , JsonSerializerOptions options)
    {
        reader.Read(); // StartObject
        reader.Read(); // StartArray

        reader.Read();
        byte A = (byte)reader.GetInt32();
        reader.Read();
        byte B = reader.GetByte();
        reader.Read();
        byte G = reader.GetByte();
        reader.Read();
        byte R = reader.GetByte();
        Color color = Color.FromArgb(A, B, G, R);
        reader.Read(); // End array

        return color;
    }

    public override void Write(Utf8JsonWriter        writer
                             , Color                 value
                             , JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteStartArray();
        writer.WriteNumberValue(value.A);
        writer.WriteNumberValue(value.B);
        writer.WriteNumberValue(value.G);
        writer.WriteNumberValue(value.R);
        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}