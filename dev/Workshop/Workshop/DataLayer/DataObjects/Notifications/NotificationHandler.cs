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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public List<MemberNotifications> Notifications { get; set; }
        public List<EventObservers> observers { get; set; }

        public NotificationHandler()
        {
        }

        public NotificationHandler(List<MemberNotifications> notifications, List<EventObservers> observers)
        {
            this.Notifications = notifications;
            this.observers = observers;
        }
    }
}
