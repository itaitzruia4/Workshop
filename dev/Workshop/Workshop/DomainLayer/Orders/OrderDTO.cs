using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using OrderDTODAL = Workshop.DataLayer.DataObjects.Orders.OrderDTO;
using ProductDTODAL = Workshop.DataLayer.DataObjects.Market.ProductDTO;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.Orders
{
    public class OrderDTO: IPersistentObject<OrderDTODAL>
    {
        public int id { get; set; }
        public string clientName { get; set; }
        public SupplyAddress address { get; set; }
        public string storeName { get; set; }
        public List<ProductDTO> items { get; set; }
        public OrderDTODAL OrderDTODAL { get; set; }

        public OrderDTO(int id, string clientName, SupplyAddress address, string storeName, List<ProductDTO> items)
        {
            this.id = id;
            this.clientName = clientName;
            this.address = address;
            this.storeName = storeName;
            this.items = items;
            List<ProductDTODAL> products = new List<ProductDTODAL>();
            foreach(ProductDTO product in items)
            {
                products.Add(product.ToDAL());
            }
            this.OrderDTODAL = new OrderDTODAL(id, clientName, address.ToDAL(), storeName, products);
            DataHandler.getDBHandler().save(OrderDTODAL);
        }

        public OrderDTO(OrderDTODAL orderDTODAL)
        {
            this.id = orderDTODAL.id;
            this.clientName = orderDTODAL.clientName;
            this.address = new SupplyAddress(orderDTODAL.address);
            this.storeName = orderDTODAL.storeName;
            this.items = new List<ProductDTO>();
            foreach (ProductDTODAL product in orderDTODAL.items)
            {
                this.items.Add(new ProductDTO(product));
            }
            this.OrderDTODAL = orderDTODAL;
        }
        public OrderDTODAL ToDAL()
        {
            return OrderDTODAL;
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
