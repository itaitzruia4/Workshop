using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.UserPackage;

namespace Workshop.DomainLayer.UserPackage.Notifications
{
    public interface IMessageHandler
    {
        void SendNotification(UserPackage.User reciver, Notification notification);
    }
}
