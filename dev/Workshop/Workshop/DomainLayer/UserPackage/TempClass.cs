using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage.Notifications;

namespace Workshop.DomainLayer.UserPackage
{
    internal class TempClass : IMessageHandler
    {
        void IMessageHandler.SendNotification(string reciverName, Notification notification)
        {
            
        }
    }
}
