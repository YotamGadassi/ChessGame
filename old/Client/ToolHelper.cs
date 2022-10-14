using ChessBoard;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client
{
    public class ToolHelper
    {
        private Dictionary<string, BitmapImage> m_ToolToImageSource;

        public ChessToolUI CreateUITool(ITool tool)
        {
            Image img = new Image();
            string toolImgKey = createImgKey(tool);
            img.Source = m_ToolToImageSource[toolImgKey];
            ChessToolUI newTool = new ChessToolUI(img, tool);

            return newTool;
        }

        public ToolHelper()
        {
            m_ToolToImageSource = new Dictionary<string, BitmapImage>();
            loadImageSources();
        }

        private void loadImageSources()
        {
            BitmapImage src = new BitmapImage(new Uri("pack://application:,,,/Resources/b_pawn.png"));
            m_ToolToImageSource["Black_Pawn"] = src;

            src = new BitmapImage(new Uri("pack://application:,,,/Resources/w_pawn.png"));
            m_ToolToImageSource["White_Pawn"] = src;

            src = new BitmapImage(new Uri("pack://application:,,,/Resources/b_rook.png"));
            m_ToolToImageSource["Black_Rook"] = src;

            src = new BitmapImage(new Uri("pack://application:,,,/Resources/w_rook.png"));
            m_ToolToImageSource["White_Rook"] = src;

            src = new BitmapImage(new Uri("pack://application:,,,/Resources/w_bishop.png"));
            m_ToolToImageSource["White_Bishop"] = src;

            src = new BitmapImage(new Uri("pack://application:,,,/Resources/b_bishop.png"));
            m_ToolToImageSource["Black_Bishop"] = src;

            src = new BitmapImage(new Uri("pack://application:,,,/Resources/b_knight.png"));
            m_ToolToImageSource["Black_Knight"] = src;

            src = new BitmapImage(new Uri("pack://application:,,,/Resources/w_knight.png"));
            m_ToolToImageSource["White_Knight"] = src;

            src = new BitmapImage(new Uri("pack://application:,,,/Resources/b_queen.png"));
            m_ToolToImageSource["Black_Queen"] = src;

            src = new BitmapImage(new Uri("pack://application:,,,/Resources/w_queen.png"));
            m_ToolToImageSource["White_Queen"] = src;

            src = new BitmapImage(new Uri("pack://application:,,,/Resources/b_king.png"));
            m_ToolToImageSource["Black_King"] = src;

            src = new BitmapImage(new Uri("pack://application:,,,/Resources/w_king.png"));
            m_ToolToImageSource["White_King"] = src;

        }

        private string createImgKey(ITool tool)
        {
            string key = createImgKey(tool.Type, tool.Color);

            return key;
        }

        private string createImgKey(string toolType, Color Color)
        {
            string colorName;

            if (Color == Colors.Black)
            {
                colorName = "Black";
            }
            else if (Color == Colors.White)
            {
                colorName = "White";
            }
            else
            {
                throw new ArgumentException(String.Format("There isn't a tool image for tool:{0} color:{1}", toolType, Color));
            }

            string key = string.Format("{0}_{1}", colorName, toolType);
            return key;
        }

        public void AddToolImage(string ToolType, Color Color, BitmapImage Image)
        {
            string key = createImgKey(ToolType, Color);
            m_ToolToImageSource.Add(key, Image);
        }

    }
}
