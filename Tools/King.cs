﻿using System.Text.Json.Serialization;
using System.Text.Json;
using System.Windows.Media;

namespace Tools
{
    [JsonConverter(typeof(KingConverter))]
    public class King : ITool
    {
        public ToolId ToolId { get; }
        public Color  Color  { get; }

        public string Type => "King";

        public King(Color color)
        {
            ToolId = ToolId.NewToolId();
            Color  = color;
        }

        internal King(Color  color
                    , ToolId toolId)
        {
            ToolId = toolId;
            Color  = color;
        }

        public ITool GetCopy()
        {
            return new King(Color);
        }

        protected bool Equals(King other)
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

            return Equals((King)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ToolId, Color);
        }
    }

    
    public class KingConverter : JsonConverter<King>
    {
        public override King? Read(ref Utf8JsonReader    reader
                                 , Type                  typeToConvert
                                 , JsonSerializerOptions options)
        {
            (Color color, ToolId? toolId) result = ToolSerializerHelper.Read(ref reader, options);
            return new King(result.color, result.toolId);
        }

        public override void Write(Utf8JsonWriter        writer
                                 , King                  value
                                 , JsonSerializerOptions options)
        {
            ToolSerializerHelper.Write(writer, value, options);
        }
    }
}
