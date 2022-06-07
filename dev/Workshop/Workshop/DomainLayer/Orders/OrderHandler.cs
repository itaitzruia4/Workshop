using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.Orders
{
    class OrderHandler<T>: IPersistentObject<DataLayer.DataObjects.Orders.OrderHandler<T>>
    {
        private Dictionary<T, List<OrderDTO>> orders;
        private static int CURR_ID = 0;
        public DataLayer.DataObjects.Orders.OrderHandler<T> OrderHandlerDAL { get; set; }

        public OrderHandler()
        {
            this.orders = new Dictionary<T, List<OrderDTO>>();
            this.OrderHandlerDAL = new DataLayer.DataObjects.Orders.OrderHandler<T>(new List<DataLayer.DataObjects.Orders.MemberToOrders<T>>());
            DataHandler.getDBHandler().save(OrderHandlerDAL);
        }

        public OrderHandler(DataLayer.DataObjects.Orders.OrderHandler<T> orderHandlerDAL)
        {
            this.orders = new Dictionary<T, List<OrderDTO>>();
            foreach(DataLayer.DataObjects.Orders.MemberToOrders<T> memberToOrders in orderHandlerDAL.MemberToOrders)
            {
                List<OrderDTO> orders = new List<OrderDTO>();
                foreach(DataLayer.DataObjects.Orders.OrderDTO orderDTO in memberToOrders.orders)
                {
                    orders.Add(new OrderDTO(orderDTO));
                }
                this.orders.Add(memberToOrders.key, orders);
            }
            this.OrderHandlerDAL = orderHandlerDAL;
        }

        public DataLayer.DataObjects.Orders.OrderHandler<T> ToDAL()
        {
            return OrderHandlerDAL;
        }

        public void addOrder(OrderDTO order, T key)
        {
            if (!this.orders.ContainsKey(key))
            {
                orders.Add(key, new List<OrderDTO>());
            }

            orders[key].Add(order);
        }

        public OrderDTO CreateOrder(string membername, SupplyAddress address, string storeName, List<ProductDTO> items)
        {
            OrderDTO temp = new OrderDTO(CURR_ID, membername, address, storeName, items);
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
