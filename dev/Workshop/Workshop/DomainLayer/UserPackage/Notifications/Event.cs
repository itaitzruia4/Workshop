using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysUser = Workshop.DomainLayer.UserPackage.User;
using EventDAL = Workshop.DataLayer.DataObjects.Notifications.Event;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.UserPackage.Notifications
{
    public class Event: IPersistentObject<EventDAL>
    {
        public string Message { get; }
        public string Name { get; }
        public string Sender { get; }
        public EventDAL EventDAL { get; set; }


        public Event(string name, string message, string sender)
        {
            Name = name;
            Message = message;
            Sender = sender;
            this.EventDAL = new EventDAL(name, message, sender);
            DataHandler.getDBHandler().save(EventDAL);
        }

        public Event(EventDAL eventDAL)
        {
            this.Message = eventDAL.Message;
            this.Sender = eventDAL.Sender;
            this.Name = eventDAL.Name;
            this.EventDAL = eventDAL;
        }


        public EventDAL ToDAL()
        {
            return EventDAL;
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() == typeof(Event))
            {
                Event eventObj = (Event) obj;
                return eventObj.Name == Name & eventObj.Sender == Sender;
            }
            return base.Equals(obj);
        }

    }
}
