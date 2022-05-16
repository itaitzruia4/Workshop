using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysUser = Workshop.DomainLayer.UserPackage.User;

namespace Workshop.DomainLayer.UserPackage.Notifications
{
    class NotificationHandler
    {
        private Dictionary<UserPackage.User, List<Notification>>  Notifications { get;}
        private IMessageHandler MessageHandler;
        private Dictionary<Event,List<SysUser>> observers = new Dictionary<Event, List<SysUser>>();
        private ILoginChecker LoginChecker;

        public NotificationHandler(IMessageHandler messageHandler, ILoginChecker loginChecker)
        {
            this.Notifications = new Dictionary<UserPackage.User, List<Notification>>();
            this.MessageHandler = messageHandler;
            this.LoginChecker = loginChecker;
            //LoginEvent = new Event("Login", "ErrorOnlyForType", "UserController");
            //observers.Add(loginEvent,new List<SysUser>());
            //observers[loginEvent].Add()
        }


        // The subscription management methods.
        public void Attach(SysUser observer, Event eventt)
        {
            if(!observers.ContainsKey(eventt))
                observers.Add(eventt, new List<SysUser>());
            this.observers[eventt].Add(observer);
        }

        public void Detach(SysUser observer, Event eventt)
        {
            this.observers[eventt].Remove(observer);
        }

        //called from login in UC
        public void LoginEvent(SysUser observer)
        {
            if (Notifications.ContainsKey(observer))
            {
                foreach (Notification notification in this.Notifications[observer])
                {
                    MessageHandler.SendNotification(observer, notification);
                }
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
                    if (!this.Notifications.ContainsKey(observer))
                    {
                        Notifications.Add(observer, new List<Notification>());
                    }
                    this.Notifications[observer].Add(new Notification(eventt, time));
                }
            }
        }
    }
}
