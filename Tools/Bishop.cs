using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Tools
{
    [JsonConverter(typeof(BishopConverter))]
    public class Bishop : ITool
    {
        public ToolId ToolId { get; }
        public Color  Color  { get; }
        public string Type   => "Bishop";

        public Bishop(Color color)
        {
            ToolId = ToolId.NewToolId();
            Color  = color;
        }

        internal Bishop(Color  color
                     , ToolId toolId)
        {
            Color  = color;
            ToolId = toolId;
        }

        public ITool GetCopy()
        {
            return new Bishop(Color);
        }

        protected bool Equals(Bishop other)
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

            return Equals((Bishop)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ToolId, Color);
        }
    }

    public class BishopConverter : JsonConverter<Bishop>
    {
        public override Bishop? Read(ref Utf8JsonReader     reader
                                   , Type                   typeToConvert
                                   , JsonSerializerOptions? options)
        {
            (Color color, ToolId? toolId) result = ToolSerializerHelper.Read(ref reader, options);
            return new Bishop(result.color, result.toolId);
        }

        public override void Write(Utf8JsonWriter        writer
                                 , Bishop                value
                                 , JsonSerializerOptions options)
        {
            ToolSerializerHelper.Write(writer, value, options);
        }
    }

}
