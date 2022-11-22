using System.Text.Json;
using System.Windows.Media;
using Common;
using Tools;

namespace JsonTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToolsTest()
        {
            ToolTest<Pawn>();
            ToolTest<Bishop>();
            ToolTest<King>();
            ToolTest<Knight>();
            ToolTest<Queen>();
            ToolTest<Rook>();
        }

        private void ToolTest<TOOL_TYPE>() where TOOL_TYPE : class, ITool
        {
            TOOL_TYPE tool          = (TOOL_TYPE)Activator.CreateInstance(typeof(TOOL_TYPE), Colors.White);
            ITool     toolInterface = tool;

            JsonSerializerOptions opt                     = new() { Converters = { new IToolConverter() } };
            string                str                     = JsonSerializer.Serialize(toolInterface, opt);
            ITool                 toolInterfaceSerialized = (ITool)JsonSerializer.Deserialize(str, typeof(ITool), opt);
            TOOL_TYPE             toolSerialized          = toolInterfaceSerialized as TOOL_TYPE;
            Assert.NotNull(toolSerialized);
            Assert.AreEqual(toolSerialized.Color, tool.Color);
            Assert.AreEqual(toolSerialized.Type, toolSerialized.Type);
        }

        [Test]
        public void BoardStateTest()
        {
            BoardState boardState = new();

            int               pairsNum  = 2;
                BoardPosition positionA = new(5, 7);
            ITool             toolA     = new Pawn(Colors.Black);

            BoardPosition positionB = new(7, 5);
            ITool         toolB     = new Pawn(Colors.White);

            boardState.Add(positionA, toolA);
            boardState.Add(positionB, toolB);

            JsonSerializerOptions opt = new() { Converters = { new IToolConverter(), new BoardStateJsonConverter() } };
            string                str = JsonSerializer.Serialize(boardState, opt);
            BoardState            serializedBoardState = (BoardState)JsonSerializer.Deserialize(str,typeof(BoardState), opt);

            Assert.AreEqual(serializedBoardState.Count, boardState.Count);
            Assert.AreEqual(serializedBoardState.Count, pairsNum);
            Assert.AreEqual(pairsNum,                   boardState.Count);

            KeyValuePair<BoardPosition, ITool>[] serializedPairs = serializedBoardState.ToArray();
            KeyValuePair<BoardPosition, ITool>[] pairs = boardState.ToArray();


            for (int i=0; i < serializedPairs.Length; ++i)
            {
                Assert.AreEqual(pairs[i].Key,         serializedPairs[i].Key);
                Assert.AreEqual(pairs[i].Value.Color, serializedPairs[i].Value.Color);
                Assert.AreEqual(pairs[i].Value.Type, serializedPairs[i].Value.Type);
            }
        }
    }
}