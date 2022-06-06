using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Members;

namespace Workshop.DataLayer.DataObjects.Notifications
{
    public class MemberNotifications
    {
        public int Id { get; set; }
        public Member Member { get; set; }
        public List<Notification> Notifications { get; set; }

        public MemberNotifications()
        {
        }
    }
}
