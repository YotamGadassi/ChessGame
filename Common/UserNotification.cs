using System;

namespace Common
{
    public enum UserNotificationStyle
    {
        NoButtons  = 0
      , OneButton  = 1
      , TwoButtons = 2
    }

    public class UserNotification
    {
        public UserNotificationStyle Style          { get; }
        public string                Notification   { get; }
        public Action[]              ButtonsActions { get; }

        public UserNotification(UserNotificationStyle style
                               , string               notification
                               , Action[]             buttonsActions)
        {
            Style               = style;
            Notification        = notification;
            ButtonsActions = buttonsActions;
        }
    }
}
