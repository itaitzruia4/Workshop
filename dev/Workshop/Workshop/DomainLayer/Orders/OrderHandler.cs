using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.Orders
{
    class OrderHandler<T>
    {
        private Dictionary<T, List<OrderDTO>> orders;

        public OrderHandler()
        {
            this.orders = new Dictionary<T, List<OrderDTO>>();
        }

        public void addOrder(OrderDTO order, T key)
        {
            if (!this.orders.ContainsKey(key))
            {
                orders.Add(key, new List<OrderDTO>());
            }

            orders[key].Add(order);
        }

        public OrderDTO findOrder(int orderID, T key)
        {
            if (!this.orders.ContainsKey(key))
            {
                return null;
            }

            foreach(OrderDTO order in this.orders[key])
            {
                if (order.Id == orderID)
                    return order;
            }

            return null;
        }
    }
}
