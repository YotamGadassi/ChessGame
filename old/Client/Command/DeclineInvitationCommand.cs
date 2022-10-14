using System;
using System.Windows.Input;

namespace Client.Command
{
    public class DeclineInvitationCommand : BaseCommandHandler
    {
        public DeclineInvitationCommand()
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
