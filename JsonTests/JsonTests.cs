using System.Text.Json;
using System.Windows.Media;
using Board;
using Common;
using Common.Chess;
using OnlineChess.Common;
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
            ITool tool = null;
            JsonSerializerOptions options = new JsonSerializerOptions(){Converters = { new IToolConverter() } };
            SimpleSerializeTest(tool, options);

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

            JsonSerializerOptions opt                     = new() { Converters = { new IToolConverter()} };
            string                str                     = JsonSerializer.Serialize(toolInterface, opt);
            ITool                 toolInterfaceSerialized = (ITool)JsonSerializer.Deserialize(str, typeof(ITool), opt);
            TOOL_TYPE             toolSerialized          = toolInterfaceSerialized as TOOL_TYPE;
            Assert.NotNull(toolSerialized);
            Assert.AreEqual(toolSerialized.Color,  tool.Color);
            Assert.AreEqual(toolSerialized.Type,   toolSerialized.Type);
            Assert.AreEqual(toolSerialized.ToolId, toolSerialized.ToolId);

        }

        [Test]
        public void ToolIdTest()
        {
            ToolId                toolId = ToolId.NewToolId();
            JsonSerializerOptions opt    = new() { Converters = { new ToolIdConverter() } };
            SimpleSerializeTest(toolId, opt);
        }

        [Test]
        public void TeamIdTest()
        {
            TeamId                toolId = TeamId.NewTeamId();;
            JsonSerializerOptions opt    = new() { Converters = { new TeamIdConverter() } };
            SimpleSerializeTest(toolId, opt);
        }

        [Test]
        public void GameConfigTests()
        {
            GameConfig gameConfig = new GameConfig(new[]
                                                   {
                                                       new TeamConfig(true, true, GameDirection.North, "A", Colors.White
                                                                    , TeamId.NewTeamId(), TimeSpan.FromMinutes(60))
                                                     , new TeamConfig(false, false, GameDirection.South, "B"
                                                                    , Colors.Black, TeamId.NewTeamId()
                                                                    , TimeSpan.FromMinutes(60))
                                                   });
            SimpleSerializeTest(gameConfig, new JsonSerializerOptions(){Converters = { new TeamIdConverter() }});
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
                Assert.AreEqual(pairs[i].Value.Type,  serializedPairs[i].Value.Type);
                Assert.AreEqual(pairs[i].Value.ToolId,  serializedPairs[i].Value.ToolId);

            }
        }

        [Test]
        public void BoardCommandTest()
        {
            BoardCommand[] commands = new BoardCommand[]
                                      {
                                          new BoardCommand(BoardCommandType.Remove, new BoardPosition(1, 1))
                                        , new BoardCommand(BoardCommandType.Add, new BoardPosition(1, 1)
                                                         , new Bishop(Colors.White))
                                      };

            JsonSerializerOptions opt = new JsonSerializerOptions(){Converters = { new IToolConverter() }};
            ArraySerializeTest(commands, opt);

        }

        [Test]
        public void GameRequestTest()
        {
            GameRequest gameRequest = new GameRequest("ABC");

            JsonSerializerOptions opt = new JsonSerializerOptions();
            SimpleSerializeTest(gameRequest, opt);
        }

        [Test]
        public void GameRequestResultTest()
        {
            GameRequestResult gameRequestResult = new GameRequestResult(true, GameRequestId.NewGameRequestId());

            JsonSerializerOptions opt = new JsonSerializerOptions();
            SimpleSerializeTest(gameRequestResult, opt);
        }

        [Test]
        public void MoveResultTest()
        {
            MoveResult moveResult = new MoveResult(MoveResultEnum.ToolKilled, new BoardPosition(3, 4)
                                                 , new BoardPosition(5, 6), new Pawn(Colors.Black)
                                                 , new King(Colors.White));

            JsonSerializerOptions opt = new JsonSerializerOptions(){Converters = { new IToolConverter() }};
            SimpleSerializeTest(moveResult, opt);
        }

        [Test]
        public void PromotionResultTest()
        {
            PromotionResult promotionResult = new PromotionResult(new Pawn(Colors.Black), new Knight( Colors.Black), new BoardPosition(1,3), PromotionResultEnum.PromotionSucceeded);

            JsonSerializerOptions opt = new JsonSerializerOptions() { Converters = { new IToolConverter() } };
            SimpleSerializeTest(promotionResult, opt);
        }

        [Test]
        public void ToolAndTeamPairTest()
        {
            ToolAndTeamPair pair1  = new ToolAndTeamPair(ToolId.NewToolId(), TeamId.NewTeamId());
            ToolAndTeamPair pair2  = new ToolAndTeamPair(ToolId.NewToolId(), TeamId.NewTeamId());

            ToolAndTeamPair[] pairArr = new[] { pair1, pair2 };

            ArraySerializeTest(pairArr, new JsonSerializerOptions(){Converters = { new ToolIdConverter(), new TeamIdConverter() }});

        }

        public void SimpleSerializeTest<T>(T obj, JsonSerializerOptions opt)
        {
            string jsonStr = JsonSerializer.Serialize(obj, opt);

            T serializedObj = (T)JsonSerializer.Deserialize(jsonStr, typeof(T), opt);
            Assert.AreEqual(obj, serializedObj);
        }

        public void ArraySerializeTest<T>(T[] obj, JsonSerializerOptions opt)
        {
            string jsonStr = JsonSerializer.Serialize(obj, opt);

            T[] serializedObj = (T[])JsonSerializer.Deserialize(jsonStr, typeof(T[]), opt);
            Assert.IsTrue(serializedObj.SequenceEqual(obj));
        }

    }
}