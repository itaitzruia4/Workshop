using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Orders
{
    public class MemberToOrders<T>
    {
        public int Id { get; set; }
        public T key { get; set; }
        public List<OrderDTO> orders { get; set; }

        public MemberToOrders()
        { }

    }
}
