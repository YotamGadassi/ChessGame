using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Tools
{
    [JsonConverter(typeof(KnightConverter))]
    public class Knight : ITool
    {
        public string Type => "Knight";

        public ToolId ToolId { get; }
        public Color  Color  { get; }

        public Knight(Color color)
        {
            ToolId = ToolId.NewToolId();
            Color  = color;
        }

        internal Knight(Color color, ToolId toolId)
        {
            ToolId = toolId;
            Color  = color;
        }

        public ITool GetCopy()
        {
            Knight newKnight = new Knight(Color);

            return newKnight;
        }

        protected bool Equals(Knight other)
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

            return Equals((Knight)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ToolId, Color);
        }
    }

    public class KnightConverter : JsonConverter<Knight>
    {
        public override Knight? Read(ref Utf8JsonReader    reader
                                   , Type                  typeToConvert
                                   , JsonSerializerOptions options)
        {
            (Color color, ToolId? toolId) result = ToolSerializerHelper.Read(ref reader, options);
            return new Knight(result.color, result.toolId);
        }

        public override void Write(Utf8JsonWriter        writer
                                 , Knight                value
                                 , JsonSerializerOptions options)
        {
            ToolSerializerHelper.Write(writer, value, options);
        }
    }
}
