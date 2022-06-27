using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationDAL = Workshop.DataLayer.DataObjects.Notifications.Notification;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.UserPackage.Notifications
{
    public class Notification: IPersistentObject<NotificationDAL>
    {
        public string Message;
        public string Sender;
        public DateTime TimeOfEvent;
        public NotificationDAL notificationDAL { get; set; }
        public Notification(string message, string sender, DateTime dateTimeOfEvent)
        {
            this.Message = message;
            this.Sender = sender;
            this.TimeOfEvent = dateTimeOfEvent;
            this.notificationDAL = new NotificationDAL(message, sender, dateTimeOfEvent);
            DataHandler.getDBHandler().save(notificationDAL);
        }

        public Notification(Event eventt, DateTime dateTimeOfEvent)
        {
            this.Message = eventt.Message;
            this.Sender = eventt.Sender;
            this.TimeOfEvent = dateTimeOfEvent;
            this.notificationDAL = new NotificationDAL(eventt.Message,eventt.Sender, TimeOfEvent);
            DataHandler.getDBHandler().save(notificationDAL);
        }

        public Notification(NotificationDAL notificationDAL)
        {
            this.Message=notificationDAL.Message;
            this.Sender=notificationDAL.Sender;
            this.TimeOfEvent=notificationDAL.TimeOfEvent;
            this.notificationDAL = notificationDAL;
        }

        public NotificationDAL ToDAL()
        {
            return notificationDAL;
        }
    }
}
