﻿using System;
using System.Windows.Controls;

namespace Client.Command
{
    public class InviteCommand : BaseCommandHandler
    {
        public override bool CanExecute(object parameter)
        {
            return null != parameter;
        }

        public override void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
