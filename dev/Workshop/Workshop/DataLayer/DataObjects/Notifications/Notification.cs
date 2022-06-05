using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Notifications
{
    public class Notification
    {
        public string Message { get; set; }
        public string Sender { get; set; }
        public DateTime TimeOfEvent { get; set; }
    }
}
