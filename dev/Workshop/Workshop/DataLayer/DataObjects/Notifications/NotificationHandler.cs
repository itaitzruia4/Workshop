using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Members;

namespace Workshop.DataLayer.DataObjects.Notifications
{
    public class NotificationHandler
    {
        public int Id { get; set; }
        public List<MemberNotifications> Notifications { get; set; }
        public List<EventObservers> observers { get; set; }

        public NotificationHandler()
        {
        }

        public NotificationHandler(int id, List<MemberNotifications> notifications, List<EventObservers> observers)
        {
            this.Id = id;
            this.Notifications = notifications;
            this.observers = observers;
        }
    }
}
