using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysUser = Workshop.DomainLayer.UserPackage.User;

namespace Workshop.DomainLayer.UserPackage.Notifications
{
    public class Event
    {
        internal string Message { get; }
        internal string Name { get; }
        internal string Sender { get; }

        public Event(string name, string message, string sender)
        {
            Name = name;
            Message = message;
            Sender = sender;
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
