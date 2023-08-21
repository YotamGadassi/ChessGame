using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Client.Annotations;
using FrontCommon;

namespace Client.Messages
{
    public enum ButtonPressed
    {
        UnDefined = 0
      , Left      = 1
      , Right     = 2
    }
    
    public class UserMessageViewModel : DependencyObject, INotifyPropertyChanged
    {
        private Dispatcher m_dispatcher;
        private string     m_leftButtonStr;
        public string LeftButtonStr
        {
            get=> m_leftButtonStr;
            set
            {
                m_leftButtonStr = value;
                OnPropertyChanged();
            }
        }

        private string m_rightButtonStr;
        public string RightButtonStr
        {
            get => m_rightButtonStr; 
            set
            {
                m_rightButtonStr = value;
                OnPropertyChanged();
            }
        }

        private string m_msgStr;
        public string MessageStr
        {
            get => m_msgStr;
            set
            {
                m_msgStr = value;
                OnPropertyChanged();
            }
        }

        private Action m_leftButtonAction;

        public ICommand LeftButtonCommand { get; }

        private Action m_rightButtonAction;

        public ICommand RightButtonCommand { get; }

        private ButtonPressed m_buttonPressed;
        public ButtonPressed ButtonPressed
        {
            get=> m_buttonPressed;
            set
            {
                m_buttonPressed = value;
                OnPropertyChanged();
            }
        }

        public UserMessageViewModel(string  msgStr
                                   , string leftButtonStr
                                    ,Action leftButtonAction
                                    ,Action rightButtonAction = null
                                   , string rightButtonStr     = null)
        {
            m_dispatcher        = Dispatcher.CurrentDispatcher;
            MessageStr          = msgStr;
            LeftButtonStr       = leftButtonStr;
            m_leftButtonAction  = leftButtonAction;
            m_rightButtonAction = rightButtonAction;
            RightButtonStr      = rightButtonStr;

            LeftButtonCommand  = new WpfCommand(onLeftButtonClick);
            RightButtonCommand = new WpfCommand(onRightButtonClick);
        }

        private void onLeftButtonClick(object param)
        {
            ButtonPressed = ButtonPressed.Left;
            if (null != m_leftButtonAction)
            {
                m_dispatcher.Invoke(m_leftButtonAction);
            }
        }

        private void onRightButtonClick(object param)
        {
            ButtonPressed = ButtonPressed.Right;
            if (null != m_rightButtonAction)
            {
                m_dispatcher.Invoke(m_rightButtonAction);
            }
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
