using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using ChessBoard.ChessBoardEventArgs;
using ChessGame;
using Client.Helpers;
using Common;
using Color = System.Windows.Media.Color;

namespace Client.Board
{
    public class BoardViewModel : DependencyObject
    {
        private SquareViewModel                            m_selectedBoardPosition;
        private HashSet<SquareViewModel>                   m_hintedBoardPositions;
        private GameManager                                m_gameManager;
        private AvailableMovesHelper                       m_availableMovesHelper;
        private Dispatcher                                 m_viewDispatcher;
        public  Dictionary<BoardPosition, SquareViewModel> SquaresDictionary => m_squaresDictionary;

        private volatile Dictionary<BoardPosition, SquareViewModel> m_squaresDictionary;

        public event EventHandler<EventArgs> ClickCommandEvent;
        
        public BoardViewModel(GameManager gameManager)
        {
            m_viewDispatcher              =  Dispatcher.CurrentDispatcher;
            m_hintedBoardPositions = new HashSet<SquareViewModel>();
            m_gameManager                 =  gameManager;
            m_gameManager.ToolMovedEvent  += moveHandler;
            m_gameManager.ToolKilledEvent += moveHandler;
            m_availableMovesHelper        =  new AvailableMovesHelper(gameManager);
            m_squaresDictionary             =  new Dictionary<BoardPosition, SquareViewModel>();
            initSquares();
        }

        private void moveHandler(object sender, ToolMovedEventArgs e)
        {
            Action<BoardPosition, BoardPosition> act = new Action<BoardPosition, BoardPosition>(moveTool);
            m_viewDispatcher.BeginInvoke( act,e.InitialPosition, e.EndPosition);
        }

        private void initSquares()
        {
            for (int rowNumber = 1; rowNumber <= 8; ++rowNumber)
            {
                for (int colNumber = 1; colNumber <= 8; ++colNumber)
                {
                    BoardPosition pos = new BoardPosition(colNumber, rowNumber);
                    SquaresDictionary.Add(pos , new SquareViewModel(ClickCommandExecute, pos));
                }
            }

        }

        public void ClickCommandExecute(BoardPosition position, ITool tool)
        {
            Color currTeamColor          = m_gameManager.CurrentTeamTurn.Color;
            bool  isPositionToolSameTeam = null != tool && tool.Color.Equals(currTeamColor);
            bool  isSelectedPosition     = null != m_selectedBoardPosition;
            if (isPositionToolSameTeam)
            {
                clearSelectedHintedBoardPositions();
                setSelectedBoardPosition(position);
                BoardPosition[] positionToMove = getAvailablePositionsToMove(position, tool);
                setHintedBoardPosition(positionToMove);
                return;
            }

            SquareViewModel squareViewModel    = SquaresDictionary[position];
            bool            isPositionInHinted = m_hintedBoardPositions.Contains(squareViewModel);
            if (isPositionInHinted)
            {
                m_gameManager.Move(m_selectedBoardPosition.Position, position);
                return;
            }
            clearSelectedHintedBoardPositions();
        }

        private BoardPosition[] getAvailablePositionsToMove(BoardPosition position, ITool tool)
        {
            return m_availableMovesHelper.GetAvailablePositionToMove(position);
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

        private void setSelectedBoardPosition(BoardPosition position)
        {
            if(SquaresDictionary.TryGetValue(position, out SquareViewModel squareVM))
            {
                m_selectedBoardPosition = squareVM;
                squareVM.State = SquareState.Chosen;
            }
        }

        private void clearSelectedHintedBoardPositions()
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

        private void setHintedBoardPosition(BoardPosition[] positions)
        {
            foreach (var boardPosition in positions)
            {
                if(SquaresDictionary.TryGetValue(boardPosition, out SquareViewModel squareVM))
                {
                    squareVM.State = SquareState.Hinted;
                    m_hintedBoardPositions.Add(squareVM);
                }
            }
        }

        private void moveTool(BoardPosition start, BoardPosition end)
        {
            clearSelectedHintedBoardPositions();
            RemoveTool(start, out ITool toolAtStart);
            ForceAddTool(toolAtStart, end);
        }
    }

}
