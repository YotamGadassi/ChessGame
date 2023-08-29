﻿using System;
using Board;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Common;
using Tools;

namespace Client.Board;

public class BoardViewModel : DependencyObject
{
    public event EventHandler<SquareViewModel>? OnSquareClick;

    public SquareViewModel?                           m_selectedBoardPosition;
    public Dictionary<BoardPosition, SquareViewModel> SquaresDictionary => m_squaresDictionary;

    private volatile Dictionary<BoardPosition, SquareViewModel> m_squaresDictionary;
    private          HashSet<SquareViewModel>?                  m_hintedBoardPositions;
    private          IBoardEvents                               m_boardEvents;
    private          Dispatcher                                 m_dispatcher;
    public BoardViewModel(IBoardEvents boardEvents)
    {
        m_dispatcher = Dispatcher.CurrentDispatcher;
        m_hintedBoardPositions = new HashSet<SquareViewModel>();
        m_squaresDictionary    = new Dictionary<BoardPosition, SquareViewModel>();
        m_boardEvents = boardEvents;
        initSquares();
        registerToEvents();
    }

    public void AddTool(ITool         tool
                      , BoardPosition position)
    {
        bool isPositionValid = SquaresDictionary.TryGetValue(position, out SquareViewModel squareVM);
        if (false == isPositionValid)
        {
            return;
        }

        squareVM.Tool = tool;
    }

    public void RemoveTool(BoardPosition position
                         , out ITool?    tool)
    {
        bool isPositionValid = SquaresDictionary.TryGetValue(position, out SquareViewModel squareVM);
        if (false == isPositionValid)
        {
            tool = null;
            return;
        }

        tool          = squareVM.Tool;
        squareVM.Tool = null;
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
        if (null != m_selectedBoardPosition)
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
            if (SquaresDictionary.TryGetValue(boardPosition, out SquareViewModel squareVM))
            {
                squareVM.State = SquareState.Hinted;
                m_hintedBoardPositions.Add(squareVM);
            }
        }
    }

    private void registerToEvents()
    {
        m_boardEvents.ToolAddEvent += AddTool;
        m_boardEvents.ToolRemoved  += (position) => RemoveTool(position, out _);
    }

    private void initSquares()
    {
        for (int rowNumber = 1; rowNumber <= 8; ++rowNumber)
        {
            for (int colNumber = 1; colNumber <= 8; ++colNumber)
            {
                BoardPosition   pos      = new(colNumber, rowNumber);
                SquareViewModel squareVM = new(pos);
                SquaresDictionary.Add(pos, squareVM);
                squareVM.ClickEvent += onSquareClickHandler;
            }
        }
    }

    private void onSquareClickHandler(object?         sender
                                    , SquareViewModel squareVM)
    {
        OnSquareClick?.Invoke(this, squareVM);
    }
}