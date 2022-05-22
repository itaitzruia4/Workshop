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
        private ConcurrentDictionary<SysUser, List<Notification>>  Notifications { get; }
        private IMessageHandler MessageHandler;
        private ConcurrentDictionary<Event, HashSet<SysUser>> observers;
        private ILoginChecker LoginChecker;

        public NotificationHandler(IMessageHandler messageHandler, ILoginChecker loginChecker)
        {
            this.Notifications = new ConcurrentDictionary<SysUser, List<Notification>>();
            observers = new ConcurrentDictionary<Event, HashSet<SysUser>>();
            this.MessageHandler = messageHandler;
            this.LoginChecker = loginChecker;
        }

        // The subscription management methods.
        public void Attach(SysUser observer, Event eventt)
        {
            observers.TryAdd(eventt, new HashSet<SysUser>());
            this.observers[eventt].Add(observer);
        }

        public void Detach(SysUser observer, Event eventt)
        {
            HashSet<SysUser> subs;
            if (observers.TryGetValue(eventt, out subs))
            {
                subs.Remove(observer);
            }
            else
            {
                throw new ArgumentException($"User is not subscribed to event {eventt.Name}");
            }
        }


        public void TriggerEvent(Event eventt)
        {
            DateTime time = DateTime.Now;
            foreach(SysUser observer in this.observers[eventt])
            {
                if (this.LoginChecker.CheckOnlineStatus(observer))
                    MessageHandler.SendNotification(observer, new Notification(eventt, time));
                else
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

        public List<Notification> GetNotifications(SysUser user)
        {
            List<Notification> notis;
            if (this.Notifications.TryGetValue(user, out notis))
            {
                return notis;
            }
            return new List<Notification>();
        }

        public void RemoveNotifications(SysUser user)
        {
            this.Notifications.AddOrUpdate(user, new List<Notification>(), (u, old) => new List<Notification>());
        }
    }
}
