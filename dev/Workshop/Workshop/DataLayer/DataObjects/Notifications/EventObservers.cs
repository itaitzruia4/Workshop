using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Members;

namespace Workshop.DataLayer.DataObjects.Notifications
{
    public class EventObservers: DALObject
    {
        public int Id { get; set; }
        public Event Event { get; set; }
        public List<Member> Observers { get; set; }

        public EventObservers()
        {
        }

        public EventObservers(Event Event, List<Member> observers)
        {
            this.Event = Event;
            this.Observers = observers;
        }
    }
}
