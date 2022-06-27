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
    public class EventObserversToMembers : DALObject
    {
        public static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public Member member { get; set; }
        public EventObservers EventObserver { get; set; }

        public EventObserversToMembers()
        {
            this.Id = nextId;
            nextId++;
        }

        public EventObserversToMembers(Member member, EventObservers EventObserver)
        {
            this.Id = nextId;
            nextId++;
            this.member = member;
            this.EventObserver = EventObserver;
        }
       

    }
}
