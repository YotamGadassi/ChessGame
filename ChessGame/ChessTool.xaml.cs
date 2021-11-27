using ChessBoard;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for ChessTool.xaml
    /// </summary>
    public partial class ChessToolUI : UserControl, ITool
    {
        private ITool toolModel;

        public Image toolImage;

        public ChessToolUI(Image ToolImage, ITool ToolModel)
        {
            InitializeComponent();

            toolImage = ToolImage;
            toolModel = ToolModel;

            m_mainGrid.Children.Add(toolImage);
        }

        public BoardPosition Position => toolModel.Position;

        public Color Color => toolModel.Color;

        public GameDirection MovingDirection => toolModel.MovingDirection;

        public string Type => toolModel.Type;

        public bool IsFirstMove => toolModel.IsFirstMove;

        public void Deactivate()
        {
            toolModel.Deactivate();
        }

        public ITool GetCopy()
        {
            return toolModel.GetCopy();
        }

        public bool IsMovingLegal(BoardPosition End, ITool ToolAtEndPoint)
        {
            return toolModel.IsMovingLegal(End, ToolAtEndPoint);
        }

        public void Move(BoardPosition Postion)
        {
            toolModel.Move(Postion);
        }

        public BoardPosition[] PossibleMoves(BoardPosition Start)
        {
            return toolModel.PossibleMoves(Start);
        }
    }
}
