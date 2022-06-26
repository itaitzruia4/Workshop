using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using DataHandler = Workshop.DataLayer.DataHandler;
using OrderDTODAL = Workshop.DataLayer.DataObjects.Orders.OrderDTO;
using Workshop.DataLayer.DataObjects.Orders;

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
            CURR_ID = OrderDTODAL.nextId;
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
                MemberToOrders<T> memberToOrders = new MemberToOrders<T>(key, new List<DataLayer.DataObjects.Orders.OrderDTO>());
                OrderHandlerDAL.MemberToOrders.Add(memberToOrders);
                DataHandler.getDBHandler().save(memberToOrders);
            }

            orders[key].Add(order);
            foreach (MemberToOrders<T> mto in OrderHandlerDAL.MemberToOrders)
            {
                if (mto.key.Equals(key) && !mto.orders.Contains(order.ToDAL()))
                {
                    mto.orders.Add(order.ToDAL());
                    DataHandler.getDBHandler().update(mto);
                }
            }
            DataHandler.getDBHandler().update(OrderHandlerDAL);
        }

        public OrderDTO CreateOrder(string membername, SupplyAddress address, string storeName, List<ProductDTO> items, DateTime date, double price)
        {
            OrderDTO temp = new OrderDTO(CURR_ID, membername, address, storeName, items,date,price);
            CURR_ID++;
            DataHandler.getDBHandler().update(OrderHandlerDAL);
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
