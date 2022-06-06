using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market;

namespace Workshop.DataLayer.DataObjects.Orders
{
    public class OrderDTO
    {
        public int id { get; set; }
        public string clientName { get; set; }
        public string address { get; set; }
        public string storeName { get; set; }
        public List<ProductDTO> items { get; set; }
    }
}
