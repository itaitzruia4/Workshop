using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage;
using Workshop.DomainLayer.UserPackage.Shopping;
using ShoppingCartDAL = Workshop.DataLayer.DataObjects.Market.ShoppingCart;
using DALObject = Workshop.DataLayer.DALObject;
using ShoppingBagDAL = Workshop.DataLayer.DataObjects.Market.ShoppingBag;
using DataHandler = Workshop.DataLayer.DataHandler;
using ShoppingBagProductDAL = Workshop.DataLayer.DataObjects.Market.ShoppingBagProduct;

namespace Workshop.DomainLayer.UserPackage.Shopping
{
    public class MemberShoppingCart : ShoppingCart, IPersistentObject<ShoppingCartDAL>
    {
        ShoppingCartDAL shoppingCartDAL;

        public MemberShoppingCart() : base()
        {
            shoppingCartDAL = new ShoppingCartDAL(new List<ShoppingBagDAL>());
            DataHandler.getDBHandler().save(shoppingCartDAL);
        }

        public MemberShoppingCart(ShoppingCartDAL shoppingCartDAL)
        {
            shoppingBags = new Dictionary<int, ShoppingBag>();
            foreach (ShoppingBagDAL shoppingBagDAL in shoppingCartDAL.ShoppingBags)
            {
                shoppingBags.Add(shoppingBagDAL.StoreId, new MemberShoppingBag(shoppingBagDAL));
            }
            this.shoppingCartDAL = shoppingCartDAL;
        }


        public override ShoppingBagProduct AddToCart(ShoppingBagProduct product, int storeId)
        {
            if (!checkIfStoreHasBag(storeId))
            {
                shoppingBags.Add(storeId, new ShoppingBag(storeId));
                shoppingCartDAL.ShoppingBags.Add(new ShoppingBagDAL(storeId, new List<ShoppingBagProductDAL>()));
            }

            ShoppingBagProduct productRet = shoppingBags[storeId].addToBag(product);
            DataHandler.getDBHandler().update(shoppingCartDAL);
            return productRet;
        }

        internal override void Clear()
        {
            base.Clear();
            foreach (ShoppingBagDAL sbDAL in shoppingCartDAL.ShoppingBags)
            {
                foreach (ShoppingBagProductDAL sbpDAL in sbDAL.Products)
                {
                    DataHandler.getDBHandler().remove(sbpDAL);
                }
                DataHandler.getDBHandler().remove(sbDAL);
            }
            shoppingCartDAL.ShoppingBags = new List<ShoppingBagDAL>();
        }

        public ShoppingCartDAL ToDAL()
        {
            return shoppingCartDAL;
        }
    }
}
