using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Media;
using Common;
using Tools;

namespace Tests
{
    internal class JsonTests
    {
        public void TestTools()
        {
            Pawn  p  = new Pawn(Colors.White);
            ITool ip = p;

            JsonSerializerOptions opt = new JsonSerializerOptions() { Converters = { new IToolConverter()} };
            string   pawnSerialized = JsonSerializer.Serialize(ip, opt);
            ITool    dp             = (ITool)JsonSerializer.Deserialize<ITool>(pawnSerialized, opt);

        }
    }
}
