using System.Windows.Media;
using Common_6;
using Common_6.ChessBoardEventArgs;
using Tools;

namespace ChessGame
{
    public class OfflineGameManager : BaseGameManager
    {
        protected override void toolKilledHandler(object sender, KillingEventArgs   e)
        {
            switchCurrentTeam();
            // need to handle:
            //1. king killed - CheckMate
            ITool killedTool = e.KilledTool;
            if (killedTool is King)
            {
                OnCheckmateEvent();
                return;
            }
            //2. promotion
            ITool movedTool = e.MovedTool;
            if (movedTool is Pawn)
            {
                if ((movedTool.Color == Colors.White && e.EndPosition.Row == 7)
                 || (movedTool.Color == Colors.Black && e.EndPosition.Row == 0))
                {
                    PromotionEventArgs promotionEventArgs = new PromotionEventArgs(e.MovedTool, e.EndPosition);
                    OnPromotionEvent(promotionEventArgs);
                    return;
                }
            }

            OnToolKilledEvent(e);
        }

        protected override void toolMovedHandler(object  sender, ToolMovedEventArgs e)
        {
            switchCurrentTeam();
            // need to handle:
            // handle promotion
            ITool movedTool = e.MovedTool;
            if (movedTool is Pawn)
            {
                if ((movedTool.Color == Colors.White && e.EndPosition.Row == 7)
                 || (movedTool.Color == Colors.Black && e.EndPosition.Row == 0))
                {
                    PromotionEventArgs promotionEventArgs       = new PromotionEventArgs(e.MovedTool, e.EndPosition);
                    OnPromotionEvent(promotionEventArgs);
                    return;
                }
            }

            //TODO: handle check for check
            OnToolMovedEvent(e);
        }
    }
}
