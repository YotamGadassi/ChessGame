using System;
using System.Collections.Generic;
using System.Windows;
using ChessGame;
using Common;
using Color = System.Windows.Media.Color;

namespace Client.Board
{
    public enum SquareState
    {
        Regular = 1,
        Chosen = 2,
        Hinted = 3
    }

    public class BoardViewModel : DependencyObject
    {
        private SquareViewModel                            m_selectedBoardPosition;
        private SquareViewModel[]                          m_hintedBoardPositions;
        private GameManager                                m_gameManager;
        public  Dictionary<BoardPosition, SquareViewModel> SquaresDictionary { get; }

        public event EventHandler<EventArgs> ClickCommandEvent;

        public BoardViewModel(GameManager gameManager)
        {
            m_gameManager     = gameManager;
            SquaresDictionary = new Dictionary<BoardPosition, SquareViewModel>();
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
            bool  isSelectedPosition        = null != m_selectedBoardPosition;
            if (false == isSelectedPosition)
            {
                Color currTeamColor             = m_gameManager.CurrentTeamTurn.Color;
                bool  isPositionValidToBeChosen = null != tool && tool.Color.Equals(currTeamColor);
                if (isPositionValidToBeChosen)
                {
                    setSelectedBoardPosition(position);
                    return;
                }

                clearSelectedHintedBoardPositions();
                return;


            }
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
            if(null == m_selectedBoardPosition)
            {
                return;
            }

            m_selectedBoardPosition.State = SquareState.Regular;
            m_selectedBoardPosition       = null;

            foreach (SquareViewModel hintedBoardPosition in m_hintedBoardPositions)
            {
                hintedBoardPosition.State = SquareState.Regular;
            }

            m_hintedBoardPositions = null;
        }

        private void setHintedBoardPosition(BoardPosition[] positions)
        {
            SquareViewModel[] hintedPositions = new SquareViewModel[positions.Length];
            for (int i=0; i<positions.Length; ++i)
            {
                if(SquaresDictionary.TryGetValue(positions[i], out SquareViewModel squareVM))
                {
                    hintedPositions[i] = squareVM;
                    squareVM.State  = SquareState.Hinted;
                }
            }

            m_hintedBoardPositions = hintedPositions;
        }
    }

}
