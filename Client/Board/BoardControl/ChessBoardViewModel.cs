using System;
using Board;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Common;
using Tools;

namespace Client.Board;

public class BoardViewModel : DependencyObject, IDisposable
{
    private static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(nameof(IsEnabled)
                                                                                             , typeof(bool)
                                                                                             , typeof(BoardViewModel)
                                                                                             , new UIPropertyMetadata(true));

    public event EventHandler<SquareViewModel>? OnSquareClick;
    public Dictionary<BoardPosition, SquareViewModel> SquaresDictionary => m_squaresDictionary;

    public bool IsEnabled
    {
        get => (bool)GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    private volatile Dictionary<BoardPosition, SquareViewModel> m_squaresDictionary;
    private readonly HashSet<SquareViewModel>?                  m_hintedBoardPositions;
    private readonly IBoardEvents                               m_boardEvents;
    private readonly Dispatcher                                 m_dispatcher;
    private          SquareViewModel?                           m_selectedBoardPosition;

    public BoardViewModel(IBoardEvents boardEvents)
    {
        m_dispatcher           = Dispatcher.CurrentDispatcher;
        m_hintedBoardPositions = new HashSet<SquareViewModel>();
        m_squaresDictionary    = new Dictionary<BoardPosition, SquareViewModel>();
        m_boardEvents          = boardEvents;
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

        m_dispatcher.Invoke(() => { squareVM.Tool = tool; });
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

        ITool removedTool = null;
        m_dispatcher.Invoke(() =>
                            {
                                removedTool   = squareVM.Tool;
                                squareVM.Tool = null;
                            });
        tool = removedTool;
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
                m_dispatcher.Invoke(() =>
                                    {
                                        hintedBoardPosition.State = SquareState.Regular;
                                    });
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
                m_dispatcher.Invoke(() =>
                                    {
                                        squareVM.State = SquareState.Hinted;
                                    });
                m_hintedBoardPositions.Add(squareVM);
            }
        }
    }

    public void Dispose()
    {
        unRegisterFromEvents();
    }

    private void registerToEvents()
    {
        m_boardEvents.ToolAddEvent += AddTool;
        m_boardEvents.ToolRemoved  += onToolRemoved;
    }

    private void unRegisterFromEvents()
    {
        m_boardEvents.ToolAddEvent -= AddTool;
        m_boardEvents.ToolRemoved  -= onToolRemoved;
        unRegisterFromSquareVmEvents();
    }

    private void unRegisterFromSquareVmEvents()
    {
        foreach (SquareViewModel squareVM in SquaresDictionary.Values)
        {
            squareVM.ClickEvent -= onSquareClickHandler;
        }
    }

    private void onToolRemoved(BoardPosition position)
    {
        RemoveTool(position, out _);
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