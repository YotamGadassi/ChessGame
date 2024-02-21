using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utils;

namespace Tools;

[JsonConverter(typeof(ToolIdConverter))]
public class ToolId : BaseId
{
    public static ToolId NewToolId()
    {
        return new ToolId(Guid.NewGuid());
    }

    private ToolId(Guid id) : base(id) { }

    protected bool Equals(ToolId other)
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

        return Equals((ToolId)obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class ToolIdConverter : JsonConverter<ToolId>
{
    private readonly string m_idFieldName = "m_id";

    public override ToolId? Read(ref Utf8JsonReader    reader
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
            typeof(ToolId).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new Type[1] { typeof(Guid) });
        ToolId item = (ToolId)ctor.Invoke(new object?[1] { id });

        reader.Read(); // End Object
        return item;
    }

    public override void Write(Utf8JsonWriter        writer
                             , ToolId                value
                             , JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        FieldInfo? idField = value.GetType().BaseType.GetField(m_idFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Guid       id      = (Guid)idField.GetValue(value);
        writer.WriteBase64String(m_idFieldName, id.ToByteArray());

        writer.WriteEndObject();
    }
}