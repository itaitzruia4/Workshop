using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Orders
{
    public class OrderHandler<T>: DALObject
    {
        public static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public List<MemberToOrders<T>> MemberToOrders { get; set; }
        public static int CURR_ID { get; set; }

        public OrderHandler()
        {
            this.Id = nextId;
            nextId++;
            MemberToOrders = new List<MemberToOrders<T>>();
        }

        public OrderHandler(List<MemberToOrders<T>> memberToOrders)
        {
            MemberToOrders = memberToOrders;
            this.Id = nextId;
            nextId++;
        }
    }
}
