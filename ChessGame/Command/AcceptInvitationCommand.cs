﻿using System;
using System.Windows.Input;

namespace Client.Command
{
    public class AcceptInvitationCommand : ICommand
    {
        public AcceptInvitationCommand()
        {
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
