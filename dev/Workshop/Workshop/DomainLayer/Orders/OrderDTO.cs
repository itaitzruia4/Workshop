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
        public SupplyAddress address { get; set; }
        public int storeId { get; set; }
        public List<ProductDTO> items { get; set; }
        public DateTime date { get; set; }
        public double price { get; set; }

        public OrderDTO(int id, string clientName, SupplyAddress address, int storeId, List<ProductDTO> items, DateTime date, double price)
        {
            this.id = id;
            this.clientName = clientName;
            this.address = address;
            this.storeId = storeId;
            this.items = items;
            this.date = date;
            this.price = price;
        }

        public bool ContainsProduct(int productId){
            foreach (ProductDTO item in items){
                if (item.Id == productId)
                    return true;
            }
            return false;
        }
    }
}
