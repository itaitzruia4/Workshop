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
        public string Message { get; set; }
        public string Sender { get; set; }
        public DateTime TimeOfEvent { get; set; }

        public Notification(DomainNotification dn)
        {
            Message = dn.Message;
            Sender = dn.Sender;
            TimeOfEvent = dn.TimeOfEvent;
        }

        public Notification(string message, string sender, DateTime timeOfEvent)
        {
            Message = message;
            Sender = sender;
            TimeOfEvent = timeOfEvent;
        }
    }
}
