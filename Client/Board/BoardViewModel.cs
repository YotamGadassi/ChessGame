using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Board;
using Common;
using Tools;

namespace Client.Board
{
    public class BoardViewModel : DependencyObject
    {
        public    SquareViewModel          m_selectedBoardPosition;
        private   HashSet<SquareViewModel> m_hintedBoardPositions;

        public           Dictionary<BoardPosition, SquareViewModel> SquaresDictionary => m_squaresDictionary;
        private volatile Dictionary<BoardPosition, SquareViewModel> m_squaresDictionary;

        public BoardViewModel(SquareViewModel.SquareClickCommandExecute squareClick, SquareViewModel.SquareClickCommandCanExecute suqareCanExecute)
        {
            m_hintedBoardPositions = new HashSet<SquareViewModel>();
            m_squaresDictionary    = new Dictionary<BoardPosition, SquareViewModel>();
            initSquares(squareClick, suqareCanExecute);
        }

        public bool AddTool(ITool tool, BoardPosition position)
        {
            bool isPositionValid = SquaresDictionary.TryGetValue(position, out SquareViewModel squareVM);
            if (false == isPositionValid)
            {
                return false;
            }

            squareVM.Tool = tool;
            return true;
        }

        public bool RemoveTool(BoardPosition position, out ITool? tool)
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
            BoardPosition[] filledPositions = SquaresDictionary.Keys.ToArray();
            foreach (BoardPosition filledPosition in filledPositions)
            {
                RemoveTool(filledPosition, out _);
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
        
        private void initSquares(SquareViewModel.SquareClickCommandExecute squareExecute, SquareViewModel.SquareClickCommandCanExecute squareCanExeucte)
        {
            for (int rowNumber = 1; rowNumber <= 8; ++rowNumber)
            {
                for (int colNumber = 1; colNumber <= 8; ++colNumber)
                {
                    BoardPosition   pos      = new(colNumber, rowNumber);
                    SquareViewModel squareVM = new(squareExecute, squareCanExeucte, pos);
                    SquaresDictionary.Add(pos , squareVM);
                }
            }
        }
    }

}
