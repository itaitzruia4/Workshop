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
        public static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public Event Event { get; set; }
        public List<EventObserversToMembers> Observers { get; set; }

        public EventObservers()
        {
            this.Id = nextId;
            nextId++;
            Observers = new List<EventObserversToMembers>();
        }

        public EventObservers(Event Event, List<EventObserversToMembers> observers)
        {
            this.Event = Event;
            this.Observers = observers;
            this.Id = nextId;
            nextId++;
        }
    }
}
