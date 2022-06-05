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
        public string storeName { get; set; }
        public List<ProductDTO> items { get; set; }

        public OrderDTO(int id, string clientName, SupplyAddress address, string storeName, List<ProductDTO> items)
        {
            this.id = id;
            this.clientName = clientName;
            this.address = address;
            this.storeName = storeName;
            this.items = items;
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
