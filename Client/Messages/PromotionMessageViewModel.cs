using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Board;
using Client.Annotations;
using FrontCommon;
using Tools;

namespace Client.Messages
{
    public class PromotionMessageViewModel : INotifyPropertyChanged
    {
        public           IList<ITool>     Tools { get; }
        public           Task<ITool>      ToolAwaiter { get; }

        public ITool ChosenTool
        {
            get => m_chosenTool;
            set
            {
                m_chosenTool = value;
                OnPropertyChanged();
            }
        }
        
        private readonly ManualResetEvent m_toolChosenEvent;
        private ITool m_chosenTool;

        private string m_basicMessage = "Please choose a tool for promotion of pawn in position ";

        public string   Message           { get; }
        public ICommand SelectToolCommand { get; }

        public PromotionMessageViewModel(Color promotedToolColor, BoardPosition toolPosition)
        {
            m_toolChosenEvent = new ManualResetEvent(false);
            Tools        = new List<ITool>();
            ToolAwaiter = new Task<ITool>(() =>
                                          {
                                              m_toolChosenEvent.WaitOne();
                                              return ChosenTool;
                                          });
            initTools(promotedToolColor);
            SelectToolCommand = new WpfCommand(chooseToolExecute);
            Message           = m_basicMessage + toolPosition;
        }

        private void initTools(Color toolsColor)
        {
            Tools.Add(new Bishop(toolsColor));
            Tools.Add(new Rook(toolsColor));
            Tools.Add(new Queen(toolsColor));
            Tools.Add(new Knight(toolsColor));
        }

        private void chooseToolExecute(object param)
        {
            m_toolChosenEvent.Set();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
