using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Members;
using Workshop.DataLayer.DataObjects.Notifications;
using Workshop.DataLayer.DataObjects.Orders;
using Workshop.DataLayer.DataObjects.Reviews;

namespace Workshop.DataLayer.DataObjects.Controllers
{
    public class UserController: DALObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public ReviewHandler reviewHandler { get; set; }
        public NotificationHandler notificationHandler { get; set; }
        public OrderHandler<string> orderHandler { get; set; }
        public List<Member> members { get; set; }

        public UserController()
        {
            members = new List<Member>();
        }

        public UserController(ReviewHandler reviewHandler, NotificationHandler notificationHandler, OrderHandler<string> orderHandler, List<Member> members)
        {
            this.reviewHandler = reviewHandler;
            this.notificationHandler = notificationHandler;
            this.orderHandler = orderHandler;
            this.members = members;
        }
    }
}
