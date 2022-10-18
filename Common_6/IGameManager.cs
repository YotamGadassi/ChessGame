using System;
using System.Windows.Media;
using Common.ChessBoardEventArgs;
using Common_6;
using Common_6.ChessBoardEventArgs;

namespace Common
{
    public delegate ITool PromotionEventHandler(object sender, PromotionEventArgs e);

    public interface IGameManager
    {
        public event EventHandler<EventArgs>          CheckmateEvent;
        public event EventHandler<EventArgs>          EndGameEvent;
        public event EventHandler<EventArgs>          StartGameEvent;
        public event EventHandler<ToolMovedEventArgs> ToolMovedEvent;
        public event EventHandler<KillingEventArgs>   ToolKilledEvent;
        public event PromotionEventHandler            PromotionEvent;
        public event EventHandler<Color>              TeamSwitchEvent;
        Color                                  CurrentColorTurn { get; }

        void StartGame();
        void EndGame();
        bool TryGetTool(BoardPosition position, out ITool     tool);
        MoveResult Move(BoardPosition       start,    BoardPosition end);

    }
}
