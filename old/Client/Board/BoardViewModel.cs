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
        public SquareViewModel                            m_selectedBoardPosition;
        private HashSet<SquareViewModel>                   m_hintedBoardPositions;
        private AvailableMovesHelper                       m_availableMovesHelper;
        private Dispatcher                                 m_viewDispatcher;
        public  Dictionary<BoardPosition, SquareViewModel> SquaresDictionary => m_squaresDictionary;

        private volatile Dictionary<BoardPosition, SquareViewModel> m_squaresDictionary;

        public event EventHandler<BoardClickEventArgs> ClickCommandEvent;
        
        public BoardViewModel()
        {
            m_viewDispatcher       = Dispatcher.CurrentDispatcher;
            m_hintedBoardPositions = new HashSet<SquareViewModel>();
            m_squaresDictionary    = new Dictionary<BoardPosition, SquareViewModel>();
            initSquares();
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

    }

}
