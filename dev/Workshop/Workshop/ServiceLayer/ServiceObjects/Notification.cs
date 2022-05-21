using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainNotification = Workshop.DomainLayer.UserPackage.Notifications.Notification;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class Notification
    {
        public string Message;
        public string Sender;
        public DateTime TimeOfEvent;

        public Notification(DomainNotification dn)
        {
            Message = dn.Message;
            Sender = dn.Sender;
            TimeOfEvent = dn.TimeOfEvent;
        }
    }
}
