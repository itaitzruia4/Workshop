using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Orders
{
    public class MemberToOrders<T> : DALObject
    {
        public static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public T key { get; set; }
        public List<OrderDTO> orders { get; set; }

        public MemberToOrders()
        {
            this.Id = nextId;
            nextId++;
            orders = new List<OrderDTO>();
        }

        public MemberToOrders(T key, List<OrderDTO> orders)
        {
            this.Id = nextId;
            nextId++;
            this.orders = orders;
            this.key = key;
        }

    }
}
