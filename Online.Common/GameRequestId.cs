using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utils;

namespace OnlineChess.Common;

[JsonConverter( typeof(GameRequestIdConverter))]
public class GameRequestId : BaseId
{
    public static GameRequestId NewGameRequestId()
    {
        return new GameRequestId(Guid.NewGuid());
    }

    private GameRequestId(Guid id) : base(id) { }

    protected bool Equals(GameRequestId other)
    {
        return base.Equals(other);
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

        return Equals((GameRequestId)obj);
    }
}

public class GameRequestIdConverter : JsonConverter<GameRequestId>
{
    private readonly string m_idFieldName = "m_id";

    public override GameRequestId? Read(ref Utf8JsonReader    reader
                                      , Type                  typeToConvert
                                      , JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            reader.Read(); // Property Name

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new SerializationException(string.Format("Serialization Error: ObjectType: {0}", typeToConvert));
        }

        reader.Read(); // Property Value
        if (reader.GetString() != m_idFieldName)
        {
            throw new Exception(""); // TODO: throw meaningful exception
        }

        reader.Read(); // Property itself
        byte[] bytes = reader.GetBytesFromBase64();
        Guid?  id    = new Guid(bytes);

        ConstructorInfo? ctor =
            typeof(GameRequestId).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new Type[1] { typeof(Guid) });
        GameRequestId item = (GameRequestId)ctor.Invoke(new object?[1] { id });

        reader.Read(); // End Object
        return item;
    }

    public override void Write(Utf8JsonWriter        writer
                             , GameRequestId         value
                             , JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        FieldInfo? idField = value.GetType().BaseType.GetField(m_idFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Guid id = (Guid)idField.GetValue(value);
        writer.WriteBase64String(m_idFieldName, id.ToByteArray());

        writer.WriteEndObject();
    }
}