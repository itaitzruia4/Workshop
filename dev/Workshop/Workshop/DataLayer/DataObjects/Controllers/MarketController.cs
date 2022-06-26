using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects.Market;
using Workshop.DataLayer.DataObjects.Orders;

namespace Workshop.DataLayer.DataObjects.Controllers
{
    public class MarketController: DALObject
    {
        public static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public UserController userController { get; set; }
        public OrderHandler<int> orderHandler { get; set; }
        public List<Store> stores { get; set; }
        public int STORE_COUNT { get; set; }
        public int PRODUCT_COUNT { get; set; }

        public MarketController()
        {
            this.Id = nextId;
            nextId++;
            userController = new UserController();
            orderHandler = new OrderHandler<int>();
            stores = new List<Store>();
            STORE_COUNT = 0;
            PRODUCT_COUNT = 0;
        }

        public MarketController(UserController userController, OrderHandler<int> orderHandler, List<Store> stores, int STORE_COUNT, int PRODUCT_COUNT)
        {
            this.userController = userController;
            this.orderHandler = orderHandler;
            this.stores = stores;
            this.STORE_COUNT = STORE_COUNT;
            this.PRODUCT_COUNT = PRODUCT_COUNT;
            this.Id = nextId;
            nextId++;
        }
    }
}
