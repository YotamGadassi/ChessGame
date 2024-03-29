﻿using Board;
using Common;
using Common.Chess;
using log4net;
using OnlineChess.Common;
using Tools;

namespace OnlineChess.Game;

public class OnlineGameBoard : IBoardEvents, IBoardQuery, IDisposable
{
    private static readonly ILog s_log = LogManager.GetLogger(typeof(OnlineGameBoard));

    public event Action<ITool, BoardPosition>? ToolAddEvent;
    public event Action<BoardPosition>? ToolRemoved;

    private readonly IChessServerAgent m_serverAgent;
    private readonly BasicBoard m_board;

    public OnlineGameBoard(IChessServerAgent serverAgent, BoardState? boardState = null)
    {
        m_board = new BasicBoard();
        m_serverAgent = serverAgent;
        registerToEvents();
        if (null != boardState)
        {
            setBoardState(boardState);
        }
    }

    public bool TryGetTool(BoardPosition position
                         , out ITool tool)
    {
        return m_board.TryGetTool(position, out tool);
    }

    public bool TryGetPosition(ITool tool
                             , out BoardPosition position)
    {
        return m_board.TryGetPosition(tool, out position);
    }

    public BoardState GetBoardState()
    {
        return m_board.GetBoardState();
    }

    public Task<MoveResult> Move(BoardPosition start
                               , BoardPosition end)
    {
        s_log.DebugFormat("Move: [{0} -> {1}]", start, end);
        return m_serverAgent.SubmitMove(start, end);
    }

    public Task<PromotionResult> PromoteTool(BoardPosition position
                                           , ITool tool)
    {
        s_log.DebugFormat("Promote: [{0} | {1}]", position, tool);

        return m_serverAgent.SubmitPromote(new PromotionRequest(tool, position));
    }

    public void Dispose()
    {
        unRegisterFromEvents();
    }

    private void setBoardState(BoardState boardState)
    {
        s_log.DebugFormat("Set Boared State: [{0}]", boardState);

        foreach (KeyValuePair<BoardPosition, ITool> pair in boardState)
        {
            BoardPosition position = pair.Key;
            ITool tool = pair.Value;
            addTool(position, tool);
        }
    }

    private void registerToEvents()
    {
        m_serverAgent.BoardCommandsEvent += onBoardCommandsArrived;
    }

    private void unRegisterFromEvents()
    {
        m_serverAgent.BoardCommandsEvent -= onBoardCommandsArrived;
    }

    private void onBoardCommandsArrived(BoardCommand[] commands)
    {
        s_log.DebugFormat("BoardCommand Msg Arrived: {0}", string.Join<BoardCommand>(',', commands));

        foreach (BoardCommand boardCommand in commands)
        {
            handleBoardCommand(boardCommand);
        }
    }

    private void handleBoardCommand(BoardCommand boardCommand)
    {
        switch (boardCommand.Type)
        {
            case BoardCommandType.Add:
                {
                    addTool(boardCommand.Position, boardCommand.Tool);
                }
                break;
            case BoardCommandType.Remove:
                {
                    removeTool(boardCommand.Position);
                }
                break;
            default:
                {
                    s_log.ErrorFormat("Unknown Command Type Arrived: {0}", boardCommand);
                }
                break;
        }
    }

    private void addTool(BoardPosition position
                       , ITool tool)
    {
        m_board.Add(position, tool);
        ToolAddEvent?.Invoke(tool, position);
    }

    private void removeTool(BoardPosition position)
    {
        m_board.Remove(position);
        ToolRemoved?.Invoke(position);
    }

}