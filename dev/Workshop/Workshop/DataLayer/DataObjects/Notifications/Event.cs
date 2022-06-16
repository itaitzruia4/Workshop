﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Notifications
{
    public class Event: DALObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public string Sender { get; set; }

        public Event()
        {
        }

        public Event(string message, string name, string sender)
        {
            Message = message;
            Name = name;
            Sender = sender;
        }
    }
}
