using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common
{
    [JsonConverter(typeof(BoardStateJsonConverter))]
    public class BoardState : IDictionary<BoardPosition, ITool>
    {
        private IDictionary<BoardPosition, ITool>               m_boardState;

        public BoardState()
        {
            m_boardState = new Dictionary<BoardPosition, ITool>();
        }

        public BoardState(BoardState other)
        {
            m_boardState = new Dictionary<BoardPosition, ITool>(other.m_boardState);
        }

        public  IEnumerator<KeyValuePair<BoardPosition, ITool>> GetEnumerator()
        {
            return m_boardState.GetEnumerator();
        }

        IEnumerator IEnumerable.                                GetEnumerator()
        {
            return ((IEnumerable)m_boardState).GetEnumerator();
        }

        public void Add(KeyValuePair<BoardPosition, ITool> item)
        {
            m_boardState.Add(item);
        }

        public void Clear()
        {
            m_boardState.Clear();
        }

        public bool Contains(KeyValuePair<BoardPosition, ITool> item)
        {
            return m_boardState.Contains(item);
        }

        public void CopyTo(KeyValuePair<BoardPosition, ITool>[] array
                         , int                                  arrayIndex)
        {
            m_boardState.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<BoardPosition, ITool> item)
        {
            return m_boardState.Remove(item);
        }

        public int Count => m_boardState.Count;

        public bool IsReadOnly => m_boardState.IsReadOnly;

        public void Add(BoardPosition key
                      , ITool         value)
        {
            m_boardState.Add(key, value);
        }

        public bool ContainsKey(BoardPosition key)
        {
            return m_boardState.ContainsKey(key);
        }

        public bool Remove(BoardPosition key)
        {
            return m_boardState.Remove(key);
        }

        public bool TryGetValue(BoardPosition key
                              , out ITool     value)
        {
            return m_boardState.TryGetValue(key, out value);
        }

        public ITool this[BoardPosition key]
        {
            get => m_boardState[key];
            set => m_boardState[key] = value;
        }

        public ICollection<BoardPosition> Keys => m_boardState.Keys;

        public ICollection<ITool> Values => m_boardState.Values;
    }

    public class BoardStateJsonConverter : JsonConverter<BoardState>
    {
        public override BoardState? Read(ref Utf8JsonReader    reader
                                       , Type                  typeToConvert
                                       , JsonSerializerOptions options)
        {
            int        arrayLength;
            BoardState boardState = new();
            // reader.Read();
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "ArrayLength")
            {
                throw new SerializationException("Array length value did not serialized");
            }

            reader.Read();
            arrayLength = reader.GetInt32();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "PositionAndToolArray")
            {
                {
                    throw new SerializationException("Array length value did not serialized");
                }
            }

            reader.Read(); // Array start
            for (int i = 0; i < arrayLength; ++i)
            {
                reader.Read(); // Start Object
                reader.Read(); // Property Name
                BoardPosition position = (BoardPosition)JsonSerializer.Deserialize(ref reader, typeof(BoardPosition));
                reader.Read(); // Property Name
                ITool tool = (ITool)JsonSerializer.Deserialize(ref reader, typeof(ITool), options);
                boardState.Add(position, tool);
                reader.Read(); // End object
            }

            reader.Read(); // End Array
            reader.Read(); // End Object

            return boardState;
        }

        public override void Write(Utf8JsonWriter        writer
                                 , BoardState            value
                                 , JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("ArrayLength", value.Count);

            writer.WriteStartArray("PositionAndToolArray");
            foreach (KeyValuePair<BoardPosition, ITool> keyValuePair in value)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Position");
                JsonSerializer.Serialize(writer, keyValuePair.Key, options);
                writer.WritePropertyName("Tool");
                JsonSerializer.Serialize(writer, keyValuePair.Value, options);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();

            writer.Flush();
        }
    }
}
