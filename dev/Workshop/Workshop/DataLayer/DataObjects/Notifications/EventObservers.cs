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
    public class EventObservers: DALObject
    {
        private static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public Event Event { get; set; }
        public List<Member> Observers { get; set; }

        public EventObservers()
        {
            this.Id = nextId;
            nextId++;
        }

        public EventObservers(Event Event, List<Member> observers)
        {
            this.Event = Event;
            this.Observers = observers;
            this.Id = nextId;
            nextId++;
        }
    }
}
