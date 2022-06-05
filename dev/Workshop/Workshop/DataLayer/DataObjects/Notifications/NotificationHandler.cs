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
        private Dictionary<Member, List<Notification>> Notifications { get; set; }
        private Dictionary<Event, HashSet<Member>> observers { get; set; }
    }
}
