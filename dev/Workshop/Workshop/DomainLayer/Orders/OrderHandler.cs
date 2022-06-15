using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;

namespace Workshop.DomainLayer.Orders
{
    class OrderHandler<T>
    {
        private Dictionary<T, List<OrderDTO>> orders;
        private static int CURR_ID = 0;
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

        public OrderDTO CreateOrder(string membername, SupplyAddress address, string storeName, List<ProductDTO> items, DateTime date, double price)
        {
            OrderDTO temp = new OrderDTO(CURR_ID, membername, address, storeName, items,date,price);
            CURR_ID++;
            return temp;
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
                return new List<OrderDTO>();
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
