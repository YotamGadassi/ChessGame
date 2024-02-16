using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Tools
{
    [JsonConverter(typeof(RookConverter))]
    public class Rook : ITool
    {
        public ToolId ToolId { get; }
        public Color  Color  { get; }

        public string Type => "Rook";

        public Rook(Color color)
        {
            ToolId = ToolId.NewToolId();
            Color  = color;
        }

        internal Rook(Color color, ToolId toolId)
        {
            ToolId = toolId;
            Color  = color;
        }

        public ITool GetCopy()
        {
            return new Rook(Color);
        }
    }

    public class RookConverter : JsonConverter<Rook>
    {
        public override Rook? Read(ref Utf8JsonReader    reader
                                 , Type                  typeToConvert
                                 , JsonSerializerOptions options)
        {
            (Color color, ToolId? toolId) result = ToolSerializerHelper.Read(ref reader, options);
            return new Rook(result.color, result.toolId);
        }

        public override void Write(Utf8JsonWriter        writer
                                 , Rook                  value
                                 , JsonSerializerOptions options)
        {
            ToolSerializerHelper.Write(writer, value, options);
        }
    }
}
