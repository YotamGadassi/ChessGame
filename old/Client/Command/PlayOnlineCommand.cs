using System;
using System.Windows.Input;

namespace Client.Command
{
    public class PlayOnlineCommand : BaseCommandHandler
    {
        public bool isOnlineModeOn {get; set;}

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            OnlineModeControl onlineModeControl = parameter as OnlineModeControl;
            onlineModeControl.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
