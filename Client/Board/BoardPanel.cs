using System.Windows;

namespace Client.Board
{
    public class BoardPanel
    {
        public FrameworkElement Control   { get; }
        public BoardViewModel   BoardVm { get; }
        public BoardPanel()
        {
            Control             = new Client.BoardControl();
            BoardVm           = new BoardViewModel();
            Control.DataContext = BoardVm;
        }
    }
}
