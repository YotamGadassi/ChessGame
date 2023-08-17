using System;
using Board;
using Tools;

namespace Common;

public interface IBoardEvents
{
    event Action<ITool, BoardPosition> ToolAddEvent;
    event Action<BoardPosition> ToolRemoved;
}