using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.Orders
{
    class OrderDTO
    {
        public int Id { get; }
        public string username;
        public string store;

        public OrderDTO(int id, string username, string store)
        {
            this.Id = id;
            this.username = username;
            this.store = store;
        }
    }
}
