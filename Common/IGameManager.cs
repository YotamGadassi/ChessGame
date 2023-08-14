using System;
using System.Threading.Tasks;
using System.Windows.Media;
using Common.ChessBoardEventArgs;

namespace Common
{
    public delegate Task<ITool> PromotionEventHandler(object sender, PromotionEventArgs e);

    public interface IGameManager
    {
        public event EventHandler<CheckmateEventArgs>    CheckmateEvent;
        public event EventHandler<EventArgs>             EndGameEvent;
        public event EventHandler<EventArgs>             StartGameEvent;
        public event EventHandler<ToolMovedEventArgs>    ToolMovedEvent;
        public event EventHandler<KillingEventArgs>      ToolKilledEvent;
        public event PromotionEventHandler               PromotionEvent;
        public event EventHandler<ToolPromotedEventArgs> ToolPromotedEvent;
        public event EventHandler<Color>                 TeamSwitchEvent;
        Color                                            CurrentColorTurn { get; }
        bool                                             IsGameRunning    { get; }
        void                                             StartGame();
        void                                             EndGame();
        bool                                             TryGetTool(BoardPosition position, out ITool     tool);
        MoveResult                                       Move(BoardPosition       start,    BoardPosition end);

    }
}
