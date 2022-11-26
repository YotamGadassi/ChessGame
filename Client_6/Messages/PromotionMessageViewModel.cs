﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Client.Annotations;
using Common;
using Tools;

namespace Client.Messages
{
    public class PromotionMessageViewModel : INotifyPropertyChanged
    {
        public  IList<ITool>     Tools { get; }
        private ManualResetEvent m_toolChosenEvent;
        public  Task<ITool>      ToolAwaiter       { get; }

        private ITool m_chosenTool;
        public ITool ChosenTool
        {
            get => m_chosenTool;
            set
            {
                m_chosenTool = value;
                OnPropertyChanged();
            }
        }

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
            SelectToolCommand = new WpfCommand(ChooseToolExecute);
            Message           = m_basicMessage + toolPosition;
            ToolAwaiter.Start();
        }

        private void initTools(Color toolsColor)
        {
            Tools.Add(new Bishop(toolsColor));
            Tools.Add(new Rook(toolsColor));
            Tools.Add(new Queen(toolsColor));
            Tools.Add(new Knight(toolsColor));
        }

        private void ChooseToolExecute(object param)
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