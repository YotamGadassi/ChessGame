using System.Text.Json;
using System.Text.Json.Serialization;
using Common;

namespace Tools
{
    [JsonConverter(typeof(IToolWrpperConverter))]
    public class IToolWrapperForServer
    {
        public ITool Tool { get; }

        public IToolWrapperForServer(ITool tool)
        {
            Tool = tool;
        }
    }

    public class IToolWrpperConverter : JsonConverter<IToolWrapperForServer>
    {
        private static IToolConverter s_toolConverter = new IToolConverter();

        public override IToolWrapperForServer? Read(ref Utf8JsonReader    reader
                                                  , Type                  typeToConvert
                                                  , JsonSerializerOptions options)
        {
            ITool tool = s_toolConverter.Read(ref reader, typeToConvert, options);
            return new IToolWrapperForServer(tool);
        }

        public override void Write(Utf8JsonWriter        writer
                                 , IToolWrapperForServer value
                                 , JsonSerializerOptions options)
        {
            s_toolConverter.Write(writer, value.Tool, options);
        }
    }
}
