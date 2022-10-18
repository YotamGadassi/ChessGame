using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using ChessGame;
using Common_6;
using Common_6.ChessBoardEventArgs;

namespace Client.Board
{
    public abstract class BaseBoardViewModel : DependencyObject
    {
        public  SquareViewModel                            m_selectedBoardPosition;
        private HashSet<SquareViewModel>                   m_hintedBoardPositions;
        protected Dispatcher                                 m_dispatcher;
        protected BaseGameManager                            m_gameManager;
        public  Dictionary<BoardPosition, SquareViewModel> SquaresDictionary => m_squaresDictionary;

        private volatile Dictionary<BoardPosition, SquareViewModel> m_squaresDictionary;

        public event EventHandler<BoardClickEventArgs> ClickCommandEvent;
        
        protected BaseBoardViewModel(BaseGameManager gameManager)
        {
            m_gameManager          = gameManager;
            m_dispatcher           = Dispatcher.CurrentDispatcher;
            m_hintedBoardPositions = new HashSet<SquareViewModel>();
            m_squaresDictionary    = new Dictionary<BoardPosition, SquareViewModel>();
            initSquares();
            registerEvents();
        }

        private void registerEvents()
        {
            m_gameManager.ToolKilledEvent += moveHandler;
            m_gameManager.ToolMovedEvent  += moveHandler;
        }

        protected virtual void ClickCommandExecute(BoardPosition position, ITool? tool)
        {
            BoardClickEventArgs eventArgs = new BoardClickEventArgs(position, tool);
            ClickCommandEvent?.Invoke(this, eventArgs);
        }

        public bool ForceAddTool(ITool tool, BoardPosition position)
        {
            bool isPositionValid = SquaresDictionary.TryGetValue(position, out SquareViewModel squareVM);
            if (false == isPositionValid)
            {
                return false;
            }

            squareVM.Tool = tool;
            return true;
        }

        public bool RemoveTool(BoardPosition position, out ITool tool)
        {
            bool isPositionValid = SquaresDictionary.TryGetValue(position, out SquareViewModel squareVM);
            if (false == isPositionValid)
            {
                tool = null;
                return false;
            }

            tool          = squareVM.Tool;
            squareVM.Tool = null;
            return null != tool;
        }

        public void RemoveAllTools()
        {
            for (int row = 1; row <= 8; ++row)
            {
                for (int col = 1; col <= 8; ++col)
                {
                    RemoveTool(new BoardPosition(col, row), out _);
                }
            }
        }

        public BoardPosition SelectedBoardPosition
        {
            set
            {
                if (SquaresDictionary.TryGetValue(value, out SquareViewModel squareVM))
                {
                    m_selectedBoardPosition = squareVM;
                    squareVM.State          = SquareState.Chosen;
                }
            }
            get => m_selectedBoardPosition?.Position ?? BoardPosition.Empty;
        }

        public void ClearSelectedAndHintedBoardPositions()
        {
            if(null != m_selectedBoardPosition)
            {
                m_selectedBoardPosition.State = SquareState.Regular;
                m_selectedBoardPosition       = null;
            }

            if (null != m_hintedBoardPositions)
            {
                foreach (SquareViewModel hintedBoardPosition in m_hintedBoardPositions)
                {
                    hintedBoardPosition.State = SquareState.Regular;
                }
                m_hintedBoardPositions.Clear();
            }
        }

        public void SetHintedBoardPosition(BoardPosition[] positions)
        {
            foreach (BoardPosition boardPosition in positions)
            {
                if(SquaresDictionary.TryGetValue(boardPosition, out SquareViewModel squareVM))
                {
                    squareVM.State = SquareState.Hinted;
                    m_hintedBoardPositions.Add(squareVM);
                }
            }
        }
        
        private void initSquares()
        {
            for (int rowNumber = 1; rowNumber <= 8; ++rowNumber)
            {
                for (int colNumber = 1; colNumber <= 8; ++colNumber)
                {
                    BoardPosition   pos      = new BoardPosition(colNumber, rowNumber);
                    SquareViewModel squareVM = new SquareViewModel(ClickCommandExecute,  pos);
                    if (m_gameManager.TryGetTool(pos, out ITool tool))
                    {
                        squareVM.Tool = tool;
                    }
                    SquaresDictionary.Add(pos , new SquareViewModel(ClickCommandExecute, pos));
                }
            }
        }

        private void moveHandler(object sender, ToolMovedEventArgs e)
        {
            Action<BoardPosition, BoardPosition, ITool> act = MoveTool;
            m_dispatcher.BeginInvoke(act, e.InitialPosition, e.EndPosition, e.MovedTool);
        }

        protected void MoveTool(BoardPosition start, BoardPosition end, ITool tool)
        {
            ClearSelectedAndHintedBoardPositions();
            RemoveTool(start, out ITool toolAtStart);
            ForceAddTool(tool, end);
        }
    }

}
