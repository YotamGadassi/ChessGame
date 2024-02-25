using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utils;

namespace Common;

[JsonConverter(typeof(TeamIdConverter))]
public class TeamId : BaseId
{
    public static TeamId NewTeamId()
    {
        return new TeamId(Guid.NewGuid());
    }

    private TeamId(Guid id) : base(id) { }

    protected bool Equals(TeamId other)
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

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((TeamId)obj);
    }
}

public class TeamIdConverter : JsonConverter<TeamId>
{
    
    private readonly string m_idFieldName = "m_id";

    public override TeamId? Read(ref Utf8JsonReader    reader
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
        Guid? id    = new Guid(bytes);
        
        ConstructorInfo? ctor =
            typeof(TeamId).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new Type[1] { typeof(Guid) });
        TeamId item = (TeamId)ctor.Invoke(new object?[1] { id });

        reader.Read(); // End Object
        return item;
    }

    public override void Write(Utf8JsonWriter        writer
                             , TeamId                value
                             , JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        FieldInfo? idField = value.GetType().BaseType.GetField(m_idFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Guid       id      = (Guid)idField.GetValue(value);
        writer.WriteBase64String(m_idFieldName, id.ToByteArray());
        
        writer.WriteEndObject();
    }
}