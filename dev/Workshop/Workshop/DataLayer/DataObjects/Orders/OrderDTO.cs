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
        public static int nextId = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }
        public string clientName { get; set; }
        public SupplyAddress address { get; set; }
        public int storeId { get; set; }
        public List<ProductDTO> items { get; set; }

        public OrderDTO()
        {
            this.id = nextId;
            nextId++;
            items = new List<ProductDTO>();
        }

        public OrderDTO(int id, string clientName, SupplyAddress address, int storeId, List<ProductDTO> items)
        {
            this.id = id;
            this.clientName = clientName;
            this.address = address;
            this.storeId = storeId;
            this.items = items;
        }
    }
}
