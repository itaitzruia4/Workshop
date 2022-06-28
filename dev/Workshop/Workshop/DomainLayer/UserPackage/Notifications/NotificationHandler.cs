using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationHandlerDAL = Workshop.DataLayer.DataObjects.Notifications.NotificationHandler;
using MemberNotifications = Workshop.DataLayer.DataObjects.Notifications.MemberNotifications;
using NotificationDAL = Workshop.DataLayer.DataObjects.Notifications.Notification;
using EventObservers = Workshop.DataLayer.DataObjects.Notifications.EventObservers;
using MemberDAL = Workshop.DataLayer.DataObjects.Members.Member;
using DataHandler = Workshop.DataLayer.DataHandler;
using EventObserversToMembers = Workshop.DataLayer.DataObjects.Notifications.EventObserversToMembers;

namespace Workshop.DomainLayer.UserPackage.Notifications
{
    class NotificationHandler: IPersistentObject<NotificationHandlerDAL>
    {
        private ConcurrentDictionary<string, List<Notification>> Notifications { get; }
        private ConcurrentDictionary<string, HashSet<string>> observers;
        private ConcurrentDictionary<string, Event> eventsnames;
        private ILoginChecker LoginChecker;
        public NotificationHandlerDAL NotificationHandlerDAL { get; set; }

        public NotificationHandler(ILoginChecker loginChecker)
        {
            this.Notifications = new ConcurrentDictionary<string, List<Notification>>();
            observers = new ConcurrentDictionary<string, HashSet<string>>();
            eventsnames = new ConcurrentDictionary<string, Event>();
            this.LoginChecker = loginChecker;
            this.NotificationHandlerDAL = new NotificationHandlerDAL(new List<MemberNotifications>(), new List<EventObservers>());
            DataHandler.Instance.Value.save(NotificationHandlerDAL);
        }

        public NotificationHandler(NotificationHandlerDAL notificationHandlerDAL, ILoginChecker loginChecker)
        {
            Notifications = new ConcurrentDictionary<string, List<Notification>>();
            observers = new ConcurrentDictionary<string, HashSet<string>>();
            eventsnames = new ConcurrentDictionary<string, Event>();
            this.LoginChecker = loginChecker;
            foreach(MemberNotifications memberNotifications in notificationHandlerDAL.Notifications)
            {
                List<Notification> newNotifications = new List<Notification>();
                foreach(NotificationDAL notification in memberNotifications.Notifications)
                {
                    newNotifications.Add(new Notification(notification));
                }
                Notifications.TryAdd(memberNotifications.Member.MemberName, newNotifications);
            }

            foreach(EventObservers eventObservers in notificationHandlerDAL.observers)
            {
                HashSet<string> newObservers = new HashSet<string>();
                foreach(EventObserversToMembers eventObserversToMembers in eventObservers.Observers)
                {
                    newObservers.Add(eventObserversToMembers.member.MemberName);
                }
                observers.TryAdd(eventObservers.Event.Name, newObservers);
                eventsnames.TryAdd(eventObservers.Event.Name, new Event(eventObservers.Event));
            }

            NotificationHandlerDAL = notificationHandlerDAL;
        }

        public NotificationHandlerDAL ToDAL()
        {
            return NotificationHandlerDAL;
        }

        // The subscription management methods.
        public void Attach(string observer_name, Event eventt)
        {
            if(eventsnames.TryAdd(eventt.Name, eventt))
            {
                EventObservers eventObservers = new EventObservers(eventt.ToDAL(), new List<EventObserversToMembers>());
                DataHandler.Instance.Value.save(eventObservers);
                NotificationHandlerDAL.observers.Add(eventObservers);
                DataHandler.Instance.Value.update(NotificationHandlerDAL);
            }

            observers.TryAdd(eventt.Name, new HashSet<string>());
            this.observers[eventt.Name].Add(observer_name);

            MemberDAL memberDAL = DataHandler.Instance.Value.find<MemberDAL>(observer_name);
            if (memberDAL != null)
            {
                foreach (EventObservers eventObservers1 in NotificationHandlerDAL.observers)
                {
                    if (eventObservers1.Event.Name.Equals(eventt.Name))
                    {
                        EventObserversToMembers eotm = new EventObserversToMembers(memberDAL, eventObservers1);
                        DataHandler.Instance.Value.save(eotm);
                        eventObservers1.Observers.Add(eotm);
                        DataHandler.Instance.Value.update(eventObservers1);
                        memberDAL.EventObservers.Add(eotm);
                        DataHandler.Instance.Value.update(memberDAL);
                        break;
                    }
                }
            }
        }

        public void Detach(string observer_name, Event eventt)
        {
            HashSet<string> subs;
            if (observers.TryGetValue(eventt.Name, out subs))
            {
                subs.Remove(observer_name);
                foreach(EventObservers eventObservers in NotificationHandlerDAL.observers)
                {
                    if(eventObservers.Event.Name.Equals(eventt.Name))
                    {
                        MemberDAL memberDAL = DataHandler.Instance.Value.find<MemberDAL>(observer_name);
                        if (memberDAL != null)
                        {
                            List<EventObserversToMembers> leotm = new List<EventObserversToMembers>();
                            foreach (EventObserversToMembers eotm in memberDAL.EventObservers)
                                if (eotm.EventObserver.Event.Name.Equals(eventt.Name))
                                {
                                    leotm.Add(eotm);
                                }

                            foreach (EventObserversToMembers eotm in leotm)
                            {
                                eventObservers.Observers.Remove(eotm);
                                memberDAL.EventObservers.Remove(eotm);
                                DataHandler.Instance.Value.remove(eotm);
                            }
                            DataHandler.Instance.Value.update(eventObservers);
                            DataHandler.Instance.Value.update(memberDAL);
                            break;
                        }
                    }
                }
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


                    List<NotificationDAL> notificationsDAL = new List<NotificationDAL>();
                    if(this.Notifications.TryAdd(observer, new List<Notification>()))
                    {
                        MemberDAL memberDAL = DataHandler.Instance.Value.find<MemberDAL>(observer);
                        if (memberDAL != null)
                        {
                            MemberNotifications memberNotifications = new MemberNotifications(memberDAL, notificationsDAL);
                            DataHandler.Instance.Value.save(memberNotifications);
                            NotificationHandlerDAL.Notifications.Add(memberNotifications);
                            DataHandler.Instance.Value.update(NotificationHandlerDAL);
                        }
                    }
                    List<Notification> notis;
                    if (this.Notifications.TryGetValue(observer, out notis))
                    {
                        Notification notification = new Notification(eventt, time);
                        notis.Add(notification);
                        notificationsDAL.Add(notification.ToDAL());
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
            foreach(MemberNotifications memberNotifications in NotificationHandlerDAL.Notifications)
            {
                if(memberNotifications.Member.MemberName.Equals(user))
                {
                    foreach(NotificationDAL notification in memberNotifications.Notifications)
                    {
                        DataHandler.Instance.Value.remove(notification);
                    }
                    memberNotifications.Notifications.Clear();
                    DataHandler.Instance.Value.update(memberNotifications);
                }
            }
        }

        
    }
}
