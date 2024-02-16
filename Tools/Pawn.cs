using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Tools
{
    [JsonConverter(typeof(PawnConverter))]
    public class Pawn : ITool
    {
        public string Type   => "Pawn";
        public ToolId ToolId { get; }

        [JsonConverter(typeof(ColorConverter))]
        public Color Color { get; }

        public Pawn(Color color)
        {
            ToolId = ToolId.NewToolId();
            Color  = color;
        }

        internal Pawn(Color color, ToolId toolId)
        {
            ToolId = toolId;
            Color  = color;
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
            (Color color, ToolId? toolId) result = ToolSerializerHelper.Read(ref reader, options);
            return new Pawn(result.color, result.toolId);
        }

        public override void Write(Utf8JsonWriter        writer
                                 , Pawn                  value
                                 , JsonSerializerOptions options)
        {
            ToolSerializerHelper.Write(writer, value, options);
        }
    }
}
