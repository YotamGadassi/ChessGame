using Common;
using Game;
using System.Windows.Media;

namespace Client
{
    public class OfflineFramwork
    {
        public BoardControl boardControl;

        public GameEngine ChessGame;

        public Team WhiteTeam;

        public Team BlackTeam;

        public OfflineFramwork()
        {
            WhiteTeam = new Team("WhiteTeam", Colors.White, GameDirection.Forward);
            BlackTeam = new Team("BlackTeam", Colors.Black, GameDirection.Backward);

            ChessGame = new GameEngine(WhiteTeam, BlackTeam);

            boardControl = new BoardControl(ChessGame);
        }
    }


}
