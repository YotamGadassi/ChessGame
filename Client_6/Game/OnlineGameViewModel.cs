using System;
using System.Drawing;
using System.Reflection;
using Client.Helpers;
using Common;
using log4net;

namespace Client.Game;

public class OnlineGameViewModel : BaseGameViewModel
{
    private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    
    public TeamStatusViewModel NorthTeamStatus { get; }
    public TeamStatusViewModel SouthTeamStatus { get; }

    public  IGameManager                             m_gameManger { get; }
    private Team                                     m_localTeam;
    private Func<BoardPosition, BoardPosition, bool> m_sendMoveRequest;
    private AvailableMovesHelper                     m_availableMovesHelper;
    public OnlineGameViewModel(IGameManager                      gameManager, 
                               Team                              northTeam, 
                               Team                              southTeam, 
                               Team                              localTeam,
                               Func<BoardPosition, BoardPosition, bool> sendMoveRequest)
    {
        m_gameManger           = gameManager;
        m_availableMovesHelper = new AvailableMovesHelper(gameManager);
        m_localTeam            = localTeam;
        NorthTeamStatus        = new TeamStatusViewModel(northTeam);
        SouthTeamStatus        = new TeamStatusViewModel(southTeam);
        m_sendMoveRequest      = sendMoveRequest;
    }

    protected override void SquareClickHandler(BoardPosition position
                                             , ITool?        tool)
    {
        bool isPositionToolSameTeam = null != tool && tool.Color.Equals(m_localTeam.Color);
        if (isPositionToolSameTeam)
        {
            Board.ClearSelectedAndHintedBoardPositions();
            Board.SelectedBoardPosition = position;
            BoardPosition[] positionToMove = m_availableMovesHelper.GetAvailablePositionToMove(position);
            Board.SetHintedBoardPosition(positionToMove);
            return;
        }

        if (false == Board.SelectedBoardPosition.IsEmpty())
        {
            bool isMoveApproved = m_sendMoveRequest(Board.SelectedBoardPosition, position);
            if (isMoveApproved)
            {
                s_log.Info($"Move from {Board.SelectedBoardPosition} to {position} approved by server");
                m_gameManger.Move(Board.SelectedBoardPosition, position);
            }
            else
            {
                s_log.Info($"Move from {Board.SelectedBoardPosition} to {position} was not approved by server");
            }
        }
        Board.ClearSelectedAndHintedBoardPositions();
    }

    protected override bool SquareClickHandlerCanExecute(BoardPosition poistion
                                                       , ITool?        tool)
    {
        bool isLocalTeamTurn = m_gameManger.CurrentColorTurn.Equals(m_localTeam.Color);
        return isLocalTeamTurn;
    }
}