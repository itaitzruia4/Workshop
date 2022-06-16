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
        private static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }
        public DateTime TimeOfEvent { get; set; }

        public Notification()
        {
            this.Id = nextId;
            nextId++;
        }

        public Notification(string message, string sender, DateTime timeOfEvent)
        {
            Message = message;
            Sender = sender;
            TimeOfEvent = timeOfEvent;
            this.Id = nextId;
            nextId++;
        }
    }
}
