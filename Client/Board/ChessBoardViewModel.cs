using System;
using Board;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Tools;

namespace Client.Board;

public class ChessBoardViewModel : DependencyObject
{
    public event EventHandler<SquareViewModel> onSquareClick;

    public  SquareViewModel?          m_selectedBoardPosition;
    public Dictionary<BoardPosition, SquareViewModel> SquaresDictionary => m_squaresDictionary;
    
    private volatile Dictionary<BoardPosition, SquareViewModel> m_squaresDictionary;
    private HashSet<SquareViewModel>? m_hintedBoardPositions;

    public ChessBoardViewModel()
    {
        m_hintedBoardPositions = new HashSet<SquareViewModel>();
        m_squaresDictionary = new Dictionary<BoardPosition, SquareViewModel>();
        initSquares();
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

        tool = squareVM.Tool;
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
                squareVM.State = SquareState.Chosen;
            }
        }
        get => m_selectedBoardPosition?.Position ?? BoardPosition.Empty;
    }

    public void ClearSelectedAndHintedBoardPositions()
    {
        if (null != m_selectedBoardPosition)
        {
            m_selectedBoardPosition.State = SquareState.Regular;
            m_selectedBoardPosition = null;
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
            if (SquaresDictionary.TryGetValue(boardPosition, out SquareViewModel squareVM))
            {
                squareVM.State = SquareState.Hinted;
                m_hintedBoardPositions.Add(squareVM);
            }
        }
    }

    public void SetSquareClickableState(BoardPosition position, bool isClickable)
    {
        SquaresDictionary[position].IsClickable = isClickable;
    }

    public void SetAllSquaresClickableState(bool isClickable)
    {
        foreach (SquareViewModel squareVM in SquaresDictionary.Values)
        {
            squareVM.IsClickable = isClickable;
        }
    }

    private void initSquares()
    {
        for (int rowNumber = 1; rowNumber <= 8; ++rowNumber)
        {
            for (int colNumber = 1; colNumber <= 8; ++colNumber)
            {
                BoardPosition pos = new(colNumber, rowNumber);
                SquareViewModel squareVM = new(pos);
                SquaresDictionary.Add(pos, squareVM);
                squareVM.ClickEvent += onSquareClickHandler;
            }
        }
    }

    private void onSquareClickHandler(object? sender, SquareViewModel squareVM)
    {
        onSquareClick(this, squareVM);
    }
}