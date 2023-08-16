using System;
using System.Windows.Media;
using Common.Chess.ChessBoardEventArgs;

namespace Client.Game;

public interface IChessGameEvents
{
    public event EventHandler<CheckmateEventArgs>?    CheckmateEvent;
    public event EventHandler<EventArgs>?             EndGameEvent;
    public event EventHandler<EventArgs>?             StartGameEvent;
    public event EventHandler<ToolMovedEventArgs>?    ToolMovedEvent;
    public event EventHandler<KillingEventArgs>?      ToolKilledEvent;
    public event EventHandler<PromotionEventArgs>?    PromotionEvent;
    public event EventHandler<ToolPromotedEventArgs>? ToolPromotedEvent;
    public event EventHandler<Color>?                 TeamSwitchEvent;
}