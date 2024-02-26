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

        protected bool Equals(Pawn other)
        {
            return ToolId.Equals(other.ToolId) && Color.Equals(other.Color);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Pawn)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ToolId, Color);
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
