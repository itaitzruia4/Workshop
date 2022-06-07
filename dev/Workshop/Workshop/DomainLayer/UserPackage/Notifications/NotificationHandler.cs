using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysUser = Workshop.DomainLayer.UserPackage.User;

namespace Workshop.DomainLayer.UserPackage.Notifications
{
    class NotificationHandler
    {
        private ConcurrentDictionary<string, List<Notification>> Notifications { get; }
        private ConcurrentDictionary<string, HashSet<string>> observers;
        private ConcurrentDictionary<string, Event> eventsnames;
        private ILoginChecker LoginChecker;

        public NotificationHandler(ILoginChecker loginChecker)
        {
            this.Notifications = new ConcurrentDictionary<string, List<Notification>>();
            observers = new ConcurrentDictionary<string, HashSet<string>>();
            eventsnames = new ConcurrentDictionary<string, Event>();
            this.LoginChecker = loginChecker;
        }

        // The subscription management methods.
        public void Attach(string observer_name, Event eventt)
        {
            eventsnames.TryAdd(eventt.Name, eventt);
            observers.TryAdd(eventt.Name, new HashSet<string>());

            this.observers[eventt.Name].Add(observer_name);
        }

        public void Detach(string observer_name, Event eventt)
        {
            HashSet<string> subs;
            if (observers.TryGetValue(eventt.Name, out subs))
            {
                subs.Remove(observer_name);
            }
            else
            {
                throw new ArgumentException($"User is not subscribed to event {eventt.Name}");
            }
        }


        public void TriggerEvent(Event eventt)
        {
            DateTime time = DateTime.Now;
            if (this.eventsnames.ContainsKey(eventt.Name))
            {
                foreach (string observer in this.observers[eventt.Name])
                {
                    this.Notifications.TryAdd(observer, new List<Notification>());
                    List<Notification> notis;
                    if (this.Notifications.TryGetValue(observer, out notis))
                    {
                        notis.Add(new Notification(eventt, time));
                    }

                }
            }
        }

        public List<Notification> GetNotifications(string user)
        {
            List<Notification> notis;
            if (this.Notifications.TryGetValue(user, out notis))
            {
                return notis;
            }
            return new List<Notification>();
        }

        public void RemoveNotifications(string user)
        {
            this.Notifications.AddOrUpdate(user, new List<Notification>(), (u, old) => new List<Notification>());
        }
    }
}
