using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market;

namespace Workshop.DataLayer.DataObjects.Orders
{
    public class OrderDTO: DALObject
    {
        public int id { get; set; }
        public string clientName { get; set; }
        public SupplyAddress address { get; set; }
        public string storeName { get; set; }
        public List<ProductDTO> items { get; set; }

        public OrderDTO()
        {
        }

        public OrderDTO(int id, string clientName, SupplyAddress address, string storeName, List<ProductDTO> items)
        {
            this.id = id;
            this.clientName = clientName;
            this.address = address;
            this.storeName = storeName;
            this.items = items;
        }
    }
}
