using System.Collections;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tools;

namespace Board
{
    [JsonConverter(typeof(BoardStateJsonConverter))]
    public class BoardState : IDictionary<BoardPosition, ITool>
    {
        private readonly IDictionary<BoardPosition, ITool> m_boardState;

        public BoardState()
        {
            m_boardState = new Dictionary<BoardPosition, ITool>();
        }

        public BoardState(BoardState other)
        {
            m_boardState = new Dictionary<BoardPosition, ITool>(other.m_boardState);
        }

        public IEnumerator<KeyValuePair<BoardPosition, ITool>> GetEnumerator()
        {
            return m_boardState.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
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

        public override string ToString()
        {
            string pairToStringFunc(KeyValuePair<BoardPosition, ITool> pair) => $"Position: {pair.Key}| Tool: {pair.Value}";
            return string.Join(',', m_boardState.Select(pairToStringFunc));
        }
    }

    public class BoardStateJsonConverter : JsonConverter<BoardState>
    {
        public override BoardState? Read(ref Utf8JsonReader    reader
                                       , Type                  typeToConvert
                                       , JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                reader.Read();

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("object did not start with StartObject token");
            }

            BoardState result = new();

            reader.Read(); // Property Name
            reader.Read(); // Array Start
            reader.Read(); // Start Object or End Array

            while (reader.TokenType == JsonTokenType.StartObject)
            {
                reader.Read(); // Property Name
                BoardPosition position = (BoardPosition)JsonSerializer.Deserialize(ref reader, typeof(BoardPosition));
                reader.Read(); // Property Name
                ITool tool = (ITool)JsonSerializer.Deserialize(ref reader, typeof(ITool), options);
                result.Add(position, tool);
                reader.Read(); // End object
                reader.Read(); // Start Object or End Array
            }


            reader.Read(); // End Object
            reader.Read();

            return result;
        }

        public override void Write(Utf8JsonWriter        writer
                                 , BoardState            value
                                 , JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteStartArray("PositionAndToolArray");
            foreach ((BoardPosition key, ITool? tool) in value)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("Position");
                JsonSerializer.Serialize(writer, key, options);
                writer.WritePropertyName("Tool");
                JsonSerializer.Serialize(writer, tool, typeof(ITool), options);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}