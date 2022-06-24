using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market;

namespace Workshop.DataLayer.DataObjects.Orders
{
    public class OrderDTO: DALObject
    {
        private static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }
        public string clientName { get; set; }
        public SupplyAddress address { get; set; }
        public string storeName { get; set; }
        public List<ProductDTO> items { get; set; }

        public OrderDTO()
        {
            this.id = nextId;
            nextId++;
            items = new List<ProductDTO>();
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
