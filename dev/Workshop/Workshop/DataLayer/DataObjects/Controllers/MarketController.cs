using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market;
using Workshop.DataLayer.DataObjects.Orders;

namespace Workshop.DataLayer.DataObjects.Controllers
{
    public class MarketController
    {
        public int Id { get; set; }
        public UserController userController { get; set; }
        public OrderHandler<int> orderHandler { get; set; }
        public List<Store> stores { get; set; }
        public int STORE_COUNT { get; set; }
        public int PRODUCT_COUNT { get; set; }

        public MarketController()
        {
        }
    }
}
