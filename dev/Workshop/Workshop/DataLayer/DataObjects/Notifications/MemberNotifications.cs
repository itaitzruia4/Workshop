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
    public class MemberNotifications: DALObject
    {
        public static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public Member Member { get; set; }
        public List<Notification> Notifications { get; set; }

        public MemberNotifications()
        {
            this.Id = nextId;
            nextId++;
            Notifications = new List<Notification>();
        }

        public MemberNotifications(Member member, List<Notification> notifications)
        {
            Member = member;
            Notifications = notifications;
            this.Id = nextId;
            nextId++;
        }
    }
}
