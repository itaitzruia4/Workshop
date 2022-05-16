using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Notifications
{
    public class Notification
    {
        public string Message;
        public string Sender;
        public DateTime TimeOfEvent;
        public Notification(string message, string sender, DateTime dateTimeOfEvent)
        {
            this.Message = message;
            this.Sender = sender;
            this.TimeOfEvent = dateTimeOfEvent;
        }

        public Notification(Event eventt, DateTime dateTimeOfEvent)
        {
            this.Message = eventt.Message;
            this.Sender = eventt.Sender;
            this.TimeOfEvent = dateTimeOfEvent;
        }
    }
}
