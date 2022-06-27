using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage.Shopping;
using ShoppingBagDAL = Workshop.DataLayer.DataObjects.Market.ShoppingBag;
using DALObject = Workshop.DataLayer.DALObject;
using ShoppingBagProductDAL = Workshop.DataLayer.DataObjects.Market.ShoppingBagProduct;
using DataHandler = Workshop.DataLayer.DataHandler;

namespace Workshop.DomainLayer.UserPackage.Shopping
{
    public class MemberShoppingBag : ShoppingBag, IPersistentObject<ShoppingBagDAL>
    {
        private ShoppingBagDAL shoppingBagDAL;

        public MemberShoppingBag(int storeId) : base(storeId)
        {
            shoppingBagDAL = new ShoppingBagDAL(storeId, new List<ShoppingBagProductDAL>());
            DataHandler.Instance.Value.save(shoppingBagDAL);
        }

        public MemberShoppingBag(ShoppingBagDAL shoppingBagDAL)
        {
            storeId = shoppingBagDAL.StoreId;
            products = new Dictionary<int, ShoppingBagProduct>();
            this.shoppingBagDAL = shoppingBagDAL;
        }

        public override ShoppingBagProduct addToBag(ShoppingBagProduct product)
        {
            if (!products.ContainsKey(product.Id))
            {
                products.Add(product.Id, product);
                shoppingBagDAL.Products.Add(product.ToDAL());
                DataHandler.Instance.Value.update(shoppingBagDAL);
            }
            else
                changeQuantity(product.Id, products[product.Id].Quantity + product.Quantity);

            return product;
        }

        internal override void deleteProduct(int productId)
        {
            shoppingBagDAL.Products.Remove(products[productId].ToDAL());
            DataHandler.Instance.Value.remove(products[productId].ToDAL());
            products.Remove(productId);
            DataHandler.Instance.Value.update(shoppingBagDAL);
        }

        internal override void changeQuantity(int productId, int newQuantity)
        {
            products[productId].Quantity = newQuantity;
            foreach(ShoppingBagProductDAL productDAL in shoppingBagDAL.Products)
                if(productDAL.Id == productId)
                    productDAL.Quantity = newQuantity;
            DataHandler.Instance.Value.update(shoppingBagDAL);
        }

        public ShoppingBagDAL ToDAL()
        {
            return shoppingBagDAL;
        }
    }
}
