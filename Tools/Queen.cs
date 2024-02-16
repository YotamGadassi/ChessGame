using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Tools
{
    [JsonConverter(typeof(QueenConverter))]
    public class Queen : ITool
    {
        public string Type => "Queen";

        public ToolId ToolId { get; }

        public Color  Color  { get; }

        public Queen(Color color)
        {
            ToolId = ToolId.NewToolId();
            Color  = color;
        }

        internal Queen(Color color, ToolId toolId)
        {
            ToolId = toolId;
            Color  = color;
        }

        public ITool GetCopy()
        {
            Queen newQueen = new Queen(Color);

            return newQueen;
        }
    }

    public class QueenConverter : JsonConverter<Queen>
    {
        public override Queen? Read(ref Utf8JsonReader    reader
                                  , Type                  typeToConvert
                                  , JsonSerializerOptions options)
        {
            (Color color, ToolId? toolId) result = ToolSerializerHelper.Read(ref reader, options);
            return new Queen(result.color, result.toolId);
        }

        public override void Write(Utf8JsonWriter        writer
                                 , Queen                 value
                                 , JsonSerializerOptions options)
        {
            ToolSerializerHelper.Write(writer, value, options);
        }
    }
}
