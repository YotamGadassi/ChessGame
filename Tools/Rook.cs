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

        protected bool Equals(Rook other)
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

            return Equals((Rook)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ToolId, Color);
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
