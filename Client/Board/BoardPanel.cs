using System.Windows;
using ChessBoard;
using ChessBoard.ChessBoardEventArgs;
using ChessGame;
using Common;

namespace Client.Board
{
    public class BoardPanel
    {
        public  FrameworkElement Control { get; }
        public  BoardViewModel   BoardVm { get; }
        private GameManager      m_gameManger;
        public BoardPanel(GameManager gameManager)
        {
            m_gameManger        = gameManager;
            Control             = new BoardControl();
            BoardVm             = new BoardViewModel(gameManager);
            Control.DataContext = BoardVm;
            registerEvents();
        }

        private void registerEvents()
        {
            m_gameManger.ToolMovedEvent += gameMangerOnToolMovedEvent;
        }

        private void gameMangerOnToolMovedEvent(object sender, ToolMovedEventArgs e)
        {
            ITool movedTool       = e.MovedTool;
            BoardPosition initialPosition = e.InitialPosition;
            BoardPosition endPosition     = e.EndPosition;
            BoardVm.RemoveTool(initialPosition, out _);
            BoardVm.ForceAddTool(movedTool, endPosition);
        }

        public void UpdateBoardViewModel()
        {
            BoardVm.RemoveAllTools();
            for (int row = 1; row <= 8; ++row)
            {
                for (int col = 1; col <= 8; ++col)
                {
                    BoardPosition boardPosition = new BoardPosition(col, row);
                    bool isToolExists = m_gameManger.TryGetTool(boardPosition, out ITool tool);
                    if (false == isToolExists)
                    {
                        continue;
                    }

                    BoardVm.ForceAddTool(tool, boardPosition);
                }
            }
        }
    }
}
