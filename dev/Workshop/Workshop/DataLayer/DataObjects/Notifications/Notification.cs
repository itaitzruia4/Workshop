using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Notifications
{
    public class Notification: DALObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }
        public DateTime TimeOfEvent { get; set; }

        public Notification()
        {
        }

        public Notification(string message, string sender, DateTime timeOfEvent)
        {
            Message = message;
            Sender = sender;
            TimeOfEvent = timeOfEvent;
        }
    }
}
