using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Client.Helpers;
using Common;
using Common.ChessBoardEventArgs;
using log4net;

namespace Client.Game;

public class OnlineGameViewModel : BaseGameViewModel
{
    private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    
    private static readonly DependencyProperty NorthTeamStatusProperty = DependencyProperty.Register("NorthTeamStatus", typeof(TeamStatusViewModel), typeof(OnlineGameViewModel));
    private static readonly DependencyProperty SouthTeamStatusProperty = DependencyProperty.Register("SouthTeamStatus", typeof(TeamStatusViewModel), typeof(OnlineGameViewModel));


    public TeamStatusViewModel NorthTeamStatus
    {
        get => (TeamStatusViewModel)GetValue(NorthTeamStatusProperty);
        set => SetValue(NorthTeamStatusProperty, value);
    }

    public TeamStatusViewModel SouthTeamStatus
    {
        get => (TeamStatusViewModel)GetValue(SouthTeamStatusProperty);
        set => SetValue(SouthTeamStatusProperty, value);
    }

    private bool isWaitingForServer = false;

    public  IGameManager                                   m_gameManager;
    private Team                                           m_localTeam;
    private Func<BoardPosition, BoardPosition, Task<bool>> m_sendMoveRequest;
    private AvailableMovesHelper                           m_availableMovesHelper;
    public OnlineGameViewModel()
    {
    }

    public void StartGame(IGameManager gameManager,
                          Team                                           northTeam
                        , Team                                           southTeam
                        , Team                                           localTeam
                        , Func<BoardPosition, BoardPosition, Task<bool>> sendMoveRequest)
    {
        m_gameManager          = gameManager;
        m_availableMovesHelper = new AvailableMovesHelper(gameManager);
        Message                = null;
        m_localTeam            = localTeam;
        NorthTeamStatus        = new TeamStatusViewModel(northTeam);
        SouthTeamStatus        = new TeamStatusViewModel(southTeam);
        m_sendMoveRequest      = sendMoveRequest;
        registerEvents();
    }

    private void registerEvents()
    {
        m_gameManager.ToolPromotedEvent += onToolPromotedEvent;
        m_gameManager.ToolKilledEvent   += moveHandler;
        m_gameManager.ToolMovedEvent    += moveHandler;
    }

    public void EndGame()
    {
        Message           = null;
        m_localTeam       = null;
        NorthTeamStatus   = null;
        SouthTeamStatus   = null;
        m_sendMoveRequest = null;
        unregisterEvents();
    }

    private void unregisterEvents()
    {
        if (null != m_gameManager)
        {
            m_gameManager.ToolPromotedEvent -= onToolPromotedEvent;
            m_gameManager.ToolKilledEvent   -= moveHandler;
            m_gameManager.ToolMovedEvent    -= moveHandler;
        }
    }

    private void moveHandler(object sender, ToolMovedEventArgs e)
    {
        Action<BoardPosition, BoardPosition, ITool> act = MoveTool;
        m_dispatcher.BeginInvoke(act, e.InitialPosition, e.EndPosition, e.MovedTool);
    }

    private void onToolPromotedEvent(object?               sender
                                   , ToolPromotedEventArgs e)
    {
        Board.RemoveTool(e.ToolPosition, out _);
        Board.AddTool(e.ToolAfterPromotion, e.ToolPosition);
    }

    protected override async void SquareClickHandler(BoardPosition position
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
            isWaitingForServer = true;
            bool isMoveApproved = await m_sendMoveRequest(Board.SelectedBoardPosition, position);
            isWaitingForServer = false;
            if (isMoveApproved)
            {
                s_log.Info($"Move from {Board.SelectedBoardPosition} to {position} approved by server");
                m_gameManager.Move(Board.SelectedBoardPosition, position);
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
        if (isWaitingForServer)
        {
            return false;
        }
        bool isLocalTeamTurn = m_gameManager.CurrentColorTurn.Equals(m_localTeam.Color);
        return isLocalTeamTurn;
    }
}