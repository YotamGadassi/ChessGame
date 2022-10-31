using System;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Windows.Media;
using log4net;

namespace Common;

public interface ITool
{
    Color Color { get; }

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
        StringBuilder str = new StringBuilder();

        reader.Read(); // start
        reader.Read(); // array start
        reader.Read(); // start
        reader.Read(); // TypeProperty
        reader.Read(); // Type name
        string? typeName = reader.GetString();
        reader.Read(); // end
        reader.Read(); // start
        Type     type = Type.GetType(typeName);
        JsonNode d    = JsonNode.Parse(ref reader);
        ITool tool = (ITool)d.Deserialize(type, options);
        reader.Read(); // array end
        reader.Read(); // end
        return tool;
    }

    public override void Write(Utf8JsonWriter        writer
                             , ITool                 value
                             , JsonSerializerOptions options)
    {
        switch (value)
        {
            case null:
                JsonSerializer.Serialize(writer, (ITool)null, options);
                break;
            default:
            Type type = value.GetType();
            writer.WriteStartObject();

            writer.WriteStartArray("Array");

            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteStringValue(type.AssemblyQualifiedName);
            writer.WriteEndObject();
            
            writer.WriteStartObject();
            writer.WritePropertyName("ActualType");
            JsonSerializer.Serialize(writer, value, type, options);
            writer.WriteEndObject();

            writer.WriteEndArray();
            writer.WriteEndObject();
            
            writer.Flush();
            break;
        }
    }
}
