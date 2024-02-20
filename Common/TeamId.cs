using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utils;

namespace Common;

[JsonSerializable(typeof(TeamIdConverter))]
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
    public override TeamId? Read(ref Utf8JsonReader    reader
                               , Type                  typeToConvert
                               , JsonSerializerOptions options)
    {
        Guid? id = null;
        while (reader.Read())
        {
            if (reader.TokenType   == JsonTokenType.PropertyName
             && reader.GetString() == "m_id")
            {
                reader.Read();
                byte[] bytes = reader.GetBytesFromBase64();
                id = new Guid(bytes);
            }
        }

        ConstructorInfo? ctor =
            typeof(TeamId).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new Type[1] { typeof(Guid) });
        return (TeamId)ctor.Invoke(new object?[1] { id });
    }

    public override void Write(Utf8JsonWriter        writer
                             , TeamId                value
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