using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Common.MainWindow
{
    public class BaseMainWindowViewModel : INotifyPropertyChanged
    {
        private Control? m_currentViewModel;

        public Control? CurrentViewModel
        {
            get => m_currentViewModel;
            set
            {
                m_currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
