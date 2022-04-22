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
                if (order.id == orderID)
                    return order;
            }

            return null;
        }

        public List<OrderDTO> GetOrders(T key)
        {
            if (!orders.ContainsKey(key))
                return null;
            return orders[key];
        }

        public bool PurchasedProduct(T key, int productId){
            if (!orders.ContainsKey(key))
                return false;
            foreach (OrderDTO order in orders[key]){
                if (order.ContainsProduct(productId)){
                    return true;
                }
            }
            return false;
        }
    }
}
