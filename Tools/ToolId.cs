using System.Reflection;
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
    public override ToolId? Read(ref Utf8JsonReader    reader
                               , Type                  typeToConvert
                               , JsonSerializerOptions options)
    {
        Guid? id = null;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName
                && reader.GetString() == "m_id")
            {
                reader.Read();
                byte[] bytes = reader.GetBytesFromBase64();
                id = new Guid(bytes);
            }
        }

        ConstructorInfo? ctor = typeof(ToolId).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new Type[1] {typeof(Guid)});
        return (ToolId)ctor.Invoke(new object?[1]{id});
    }

    public override void Write(Utf8JsonWriter        writer
                             , ToolId                value
                             , JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("m_id");
        FieldInfo? idField = value.GetType().BaseType.GetField("m_id", BindingFlags.NonPublic | BindingFlags.Instance);
        Guid       id      = (Guid)idField.GetValue(value);
        writer.WriteBase64StringValue(id.ToByteArray());
        writer.WriteEndObject();
    }
}