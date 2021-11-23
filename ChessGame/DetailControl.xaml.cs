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

namespace ChessGame
{
    /// <summary>
    /// Interaction logic for DetailControl.xaml
    /// </summary>
    public partial class DetailControl : UserControl
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(DetailControl));
        public static readonly DependencyProperty TextBoxProperty = DependencyProperty.Register("TextBox", typeof(string), typeof(DetailControl));


        public string Label1
        {
            get { return (string)GetValue(LabelProperty);}
            set { SetValue(LabelProperty, value); }
        }
    
        public string TextBox1
        {
            get{ return (string)GetValue(TextBoxProperty); }
            set{ SetValue(TextBoxProperty, value); }
        }

        public DetailControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
