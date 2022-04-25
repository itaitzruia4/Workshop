using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;

namespace Workshop.DomainLayer.Orders
{
    public class OrderDTO
    {
        public int id { get; set; }
        public string clientName { get; set; }
        public string address { get; set; }
        public string storeName { get; set; }
        public List<ShoppingBagProduct> items { get; set; }
        public double totalPrice { get; set; }

        public OrderDTO(int id, string clientName, string address, string storeName, List<ShoppingBagProduct> items, double totalPrice)
        {
            this.id = id;
            this.clientName = clientName;
            this.address = address;
            this.storeName = storeName;
            this.items = items;
            this.totalPrice = totalPrice;
        }

        public bool ContainsProduct(int productId){
            foreach (ShoppingBagProduct item in items){
                if (item.productId == productId)
                    return true;
            }
            return false;
        }
    }
}
