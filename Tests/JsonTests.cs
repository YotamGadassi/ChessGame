using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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

            JsonSerializerOptions opt = new JsonSerializerOptions() { Converters = { new IToolConverter() } };

            string   pawnSerialized = JsonSerializer.Serialize(ip, opt);
            JsonNode node           = JsonNode.Parse(pawnSerialized);
            ITool    dp             = (ITool)JsonSerializer.Deserialize(node,typeof(ITool), opt);

        }
    }
}
