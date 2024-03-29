﻿using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;
using log4net;

namespace Tools;

[JsonConverter(typeof(IToolConverter))]
public interface ITool
{
    ToolId ToolId { get; }
    Color  Color  { get; }

    string Type { get; }

    ITool GetCopy();
}

public class IToolConverter : JsonConverter<ITool>
{
    private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    
    public override ITool? Read(ref Utf8JsonReader    reader
                              , Type                  typeToConvert
                              , JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            reader.Read(); // Start Object

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new SerializationException($"Serialization Error for type: {typeToConvert}");
        }

        reader.Read(); // First Property Name
        string propertyName = reader.GetString();
        if (propertyName != "Type")
        {
            throw new SerializationException("Type name did not deserialize");
        }

        reader.Read();
        string? typeName = reader.GetString();
        Type    toolType = Type.GetType(typeName);
        
        while (reader.Read() && reader.TokenType != JsonTokenType.PropertyName) { }
        propertyName = reader.GetString();
        if (propertyName != "ConcreteType")
        {
            throw new SerializationException("ConcreteType property did not serialized");
        }

        reader.Read();
        ITool tool = (ITool)JsonSerializer.Deserialize(ref reader, toolType, options);

        reader.Read(); // End Object
        return tool;
    }

    public override void Write(Utf8JsonWriter        writer
                             , ITool                 value
                             , JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (null == value)
        {
            s_log.Warn("Writing a null value!");
            JsonSerializer.Serialize(writer, (ITool)null, options);
        }
        else
        {
            Type type = value.GetType();

            writer.WriteString("Type", type.AssemblyQualifiedName);

            writer.WritePropertyName("ConcreteType");
            JsonSerializer.Serialize(writer, value, type, options);
        }

        writer.WriteEndObject();
    }
}
