using System.Windows.Threading;
using Board;
using ChessGame.Helpers;
using Client.Board;
using Client.Game.GameMainControl;
using Client.Messages;
using Common;
using log4net;
using OnlineChess.Game;
using OnlineChess.TeamManager;
using Tools;

namespace OnlineChess.UI;

public class OnlineChessViewModel : ChessGameViewModel
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineChessViewModel));

    private readonly OnlineGameBoard        m_gameBoard;
    private readonly OnlineChessTeamManager m_teamManager;
    private readonly IAvailableMovesHelper  m_availableMovesHelper;
    private readonly Dispatcher             m_dispatcher;
    public OnlineChessViewModel(OnlineChessGameManager gameManager
                              , Dispatcher             dispatcher) : base(gameManager, 
                                                                          gameManager.BoardEvents
                                                                        , gameManager.TeamsManager)
    {
        m_dispatcher           = dispatcher;
        m_gameBoard            = gameManager.GameBoard;
        m_teamManager          = gameManager.TeamsManager;
        m_availableMovesHelper = new AvailableMovesHelper(gameManager.BoardQuery);
        initBoardState(gameManager.BoardQuery);
    }

    private void initBoardState(IBoardQuery boardQuery)
    {
        m_dispatcher.Invoke(() =>
                            {
                                BoardState boardState = boardQuery.GetBoardState();
                                foreach (KeyValuePair<BoardPosition, ITool> pair in boardState)
                                {
                                    Board.AddTool(pair.Value, pair.Key);
                                }
                            });
    }

    protected override async void onSquareClick(object?         sender
                                              , SquareViewModel squareVM)
    {
        s_log.DebugFormat("Click on square: {0}", squareVM);

        bool isLocalMachineTeamTurn = m_teamManager.IsLocalMachineTeamTurn();

        if (!isLocalMachineTeamTurn)
        {
            return;
        }

        ITool         tool                = squareVM.Tool;
        BoardPosition position            = squareVM.Position;
        TeamId        localMachineTeamId  = m_teamManager.LocalMachineTeamId;
        bool          isToolBelongsToTeam = null != tool && m_teamManager.GetTeamId(tool.ToolId).Equals(localMachineTeamId);
        if (isToolBelongsToTeam)
        {
            Board.ClearSelectedAndHintedBoardPositions();
            Board.SelectedBoardPosition = position;
            BoardPosition[] availablePositionsToMove = m_availableMovesHelper.GetAvailablePositionToMove(position);
            Board.SetHintedBoardPosition(availablePositionsToMove);
            return;
        }

        if (false == Board.SelectedBoardPosition.IsEmpty())
        {
            BoardPosition start      = Board.SelectedBoardPosition;
            BoardPosition end        = position;
            await m_gameBoard.Move(start, end);
        }

        Board.ClearSelectedAndHintedBoardPositions();
    }

    protected override async void onPromotion(PromotionRequest promotionRequest)
    {
        BoardPosition position      = promotionRequest.Position;
        ITool         toolToPromote = promotionRequest.ToolToPromote;
        s_log.Info($"Promotion event: position: {position} | tool to promote: {toolToPromote}");
        PromotionMessageViewModel promotionMessage = new(toolToPromote.Color, position);
        await m_dispatcher.Invoke(
                                  async () =>
                                  {
                                      promotionMessage.ToolAwaiter.Start();
                                      Message = promotionMessage;
                                      ITool newTool = await promotionMessage.ToolAwaiter;
                                      Message = null;
                                      await m_gameBoard.PromoteTool(position, newTool);
                                  });
    }

    protected override void onCheckMate(BoardPosition position
                                      , ITool         tool)
    {
        s_log.Info($"Checkmate Event: Position:{position} | Tool:{tool}");

         UserMessageViewModel checkMateMessage = new("Checkmate", "OK", () =>
                                                                        {
                                                                            Message = null;
                                                                            m_dispatcher
                                                                               .InvokeAsync(() =>
                                                                                            {

                                                                                            });
                                                                        });
         Message = checkMateMessage;
    }
}