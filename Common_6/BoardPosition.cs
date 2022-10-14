using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common_6
{
    [JsonConverter(typeof(BoardPositionJsonConverter))]
    public readonly struct BoardPosition
    {
        public int Column { get; }
        public int Row { get; }

        public static BoardPosition Empty = new(int.MinValue, int.MinValue);

        public BoardPosition(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public bool IsEmpty()
        {
            return Equals(Empty);
        }
    }

    public class BoardPositionJsonConverter : JsonConverter<BoardPosition>
    {
        public override BoardPosition Read(ref Utf8JsonReader reader, Type          typeToConvert, JsonSerializerOptions options)
        {
            int column = -1;
            int row = -1;
            
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();
                    if (propertyName == "Column")
                    {
                        column = reader.GetInt32();
                    }
                    else if (propertyName == "Row")
                    {
                        row = reader.GetInt32();
                    }
                }
            }

            return new BoardPosition(column, row);
        }

        public override void Write(Utf8JsonWriter    writer, BoardPosition value,         JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteStartArray("BoardPosition");

            writer.WriteStartObject();
            writer.WritePropertyName("Column");
            writer.WriteNumberValue(value.Column);
            writer.WriteEndObject();

            writer.WriteStartObject();
            writer.WritePropertyName("Row");
            writer.WriteNumberValue(value.Row);
            writer.WriteEndObject();

            writer.WriteEndArray();
            writer.WriteEndObject();

            writer.Flush();
        }
    }
}
