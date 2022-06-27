using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Members;

namespace Workshop.DataLayer.DataObjects.Notifications
{
    public class NotificationHandler: DALObject
    {
        public static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public List<MemberNotifications> Notifications { get; set; }
        public List<EventObservers> observers { get; set; }

        public NotificationHandler()
        {
            this.Id = nextId;
            nextId++;
            Notifications = new List<MemberNotifications>();
            observers = new List<EventObservers>();
        }

        public NotificationHandler(List<MemberNotifications> notifications, List<EventObservers> observers)
        {
            this.Notifications = notifications;
            this.observers = observers;
            this.Id = nextId;
            nextId++;
        }
    }
}
