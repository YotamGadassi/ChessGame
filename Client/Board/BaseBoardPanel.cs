using System;
using System.Windows;
using System.Windows.Threading;
using ChessBoard.ChessBoardEventArgs;
using ChessGame;
using Client.Helpers;
using Common;

namespace Client.Board
{
    public abstract class BaseBoardPanel
    {
        protected BaseGameManager          m_gameManager;
        protected Dispatcher           m_viewDispatcher;
        protected AvailableMovesHelper m_availableMoveHelper;
        public    FrameworkElement     Control { get; }
        public    BoardViewModel       BoardVm { get; }

        protected BaseBoardPanel(BaseGameManager gameManager)
        {
            m_viewDispatcher      = Dispatcher.CurrentDispatcher;
            m_gameManager         = gameManager;
            m_availableMoveHelper = new AvailableMovesHelper(m_gameManager);
            Control               = new BoardControl();
            BoardVm               = new BoardViewModel();
            Control.DataContext   = BoardVm;
        }

        public void Init()
        {
            updateBoardViewModel();
            registerEvents();
        }

        private void registerEvents()
        {
            BoardVm.ClickCommandEvent     += clickCommand;
            m_gameManager.ToolKilledEvent += moveHandler;
            m_gameManager.ToolMovedEvent  += moveHandler;
        }

        protected void updateBoardViewModel()
        {
            BoardVm.RemoveAllTools();
            for (int row = 1; row <= 8; ++row)
            {
                for (int col = 1; col <= 8; ++col)
                {
                    BoardPosition boardPosition = new BoardPosition(col, row);
                    bool          isToolExists  = m_gameManager.TryGetTool(boardPosition, out ITool tool);
                    if (false == isToolExists)
                    {
                        continue;
                    }

                    BoardVm.ForceAddTool(tool, boardPosition);
                }
            }
        }

        protected abstract void clickCommand(object sender, BoardClickEventArgs args);

        protected void moveHandler(object sender, ToolMovedEventArgs e)
        {
            Action<BoardPosition, BoardPosition> act = moveTool;
            m_viewDispatcher.BeginInvoke(act, e.InitialPosition, e.EndPosition);
        }

        protected void moveTool(BoardPosition start, BoardPosition end)
        {
            BoardVm.ClearSelectedAndHintedBoardPositions();
            BoardVm.RemoveTool(start, out ITool toolAtStart);
            BoardVm.ForceAddTool(toolAtStart, end);
        }
    }

}
