using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Workshop.DomainLayer.UserPackage.Shopping;
using Workshop.DomainLayer.MarketPackage;
using System.Threading;
using StoreDAL = Workshop.DataLayer.DataObjects.Market.Store;
using DALObject = Workshop.DataLayer.DALObject;
using ProductDAL = Workshop.DataLayer.DataObjects.Market.Product;
using DiscountPolicyDAL = Workshop.DataLayer.DataObjects.Market.Discounts.DiscountPolicy;
using PurchasePolicyDAL = Workshop.DataLayer.DataObjects.Market.Purchases.PurchasePolicy;
using DataHandler = Workshop.DataLayer.DataHandler;

using Member = Workshop.DomainLayer.UserPackage.Permissions.Member;
using Bid = Workshop.DomainLayer.MarketPackage.Biding.Bid;

namespace Workshop.DomainLayer.MarketPackage
{
    public class Store : IPersistentObject<StoreDAL>
    {
        private bool open { get; set; }
        public readonly int id;
        private string name { get; set; }
        private Dictionary<int, Product> products { get; set; }
        private DiscountPolicy discountPolicy { get; set; }
        private PurchasePolicy purchasePolicy { get; set; }

        private ReaderWriterLock rwl;

        private StoreDAL storeDAL;


        //public Store(int id, string name)
        public HashSet<Member> owners { get; }
        public ConcurrentDictionary<int,Bid> biding_votes { get; set; }
        private volatile int bid_id_count;
        public ConcurrentDictionary<Member, KeyValuePair<Member, HashSet<Member>>> owner_voting { get; }



        public Store(int id, string name, Member founder)
        {
            if (name == null || name.Equals(""))
                throw new ArgumentException("Store name cannot be empty.");
            this.id = id;
            this.name = name;
            products = new Dictionary<int, Product>();
            this.open = true; //TODO: check if on init store supposed to be open or closed.
            this.rwl = new ReaderWriterLock();
            this.discountPolicy = new DiscountPolicy(this);
            this.purchasePolicy = new PurchasePolicy(this);

            List<ProductDAL> productsDAL = new List<ProductDAL>();
            DiscountPolicyDAL dpDAL = (DiscountPolicyDAL)discountPolicy.ToDAL();
            PurchasePolicyDAL ppDAL = (PurchasePolicyDAL)purchasePolicy.ToDAL();
            storeDAL = new StoreDAL(id, open, name, dpDAL, ppDAL, productsDAL);
            DataHandler.getDBHandler().save(storeDAL);
            open = true; //TODO: check if on init store supposed to be open or closed.
            discountPolicy = new DiscountPolicy(this);
            purchasePolicy = new PurchasePolicy(this);
            owners = new HashSet<Member>();
            owners.Add(founder);
            bid_id_count = 0;
            owner_voting = new ConcurrentDictionary<Member, KeyValuePair<Member, HashSet<Member>>>();
            biding_votes = new ConcurrentDictionary<int,Bid>();
        }

        public bool AddOwner(Member owner)
        {
            return owners.Add(owner);
        }

        public Store(StoreDAL storeDAL)
        {
            this.id = storeDAL.Id;
            this.name = storeDAL.StoreName;
            this.products = new Dictionary<int, Product>();
            this.open = storeDAL.Open;
            this.rwl = new ReaderWriterLock();
            this.discountPolicy = new DiscountPolicy(storeDAL.DiscountPolicy, this);
            this.purchasePolicy = new PurchasePolicy(storeDAL.PurchasePolicy, this);

            foreach(ProductDAL product in storeDAL.Products)
            {
                products.Add(product.Id, new Product(product));
            }
        }

        public StoreDAL ToDAL()
        {
            return storeDAL;
        }

        public ReaderWriterLock getLock()
        {
            return this.rwl;
        }

        public Member VoteForStoreOwnerNominee(Member voter, Member nominee)
        {
            // Returns the nominator of nominee if he was successfuly voted on by all parties, else returns null
            if (!owners.Contains(voter))
            {
                throw new ArgumentException($"{voter.Username} is not a store owner of store {id}, hence he can not vote to nominate {nominee.Username} as a store owner.");
            }
            if (owners.Contains(nominee))
            {
                throw new ArgumentException($"{nominee.Username} is already a store owner of store {id}, and you can not vote to nominate him again.");
            }
            KeyValuePair<Member, HashSet<Member>> nominator_voter_pair;
            if (owner_voting.TryGetValue(nominee, out nominator_voter_pair))
            {
                if (nominator_voter_pair.Value.Add(voter))
                {
                    return nominator_voter_pair.Value.Count == owners.Count ? nominator_voter_pair.Key : null;
                }
                else
                {
                    throw new ArgumentException($"{voter.Username} has already voted for {nominee.Username} to be a store owner of store {id}.");
                }
            }
            else
            {
                HashSet<Member> temp = new HashSet<Member>();
                temp.Add(voter);
                owner_voting.TryAdd(nominee, new KeyValuePair<Member, HashSet<Member>>(voter, temp));
                return 1 == owners.Count ? voter : null;
            }
        }

        internal void RejectStoreOwnerNomination(Member rejecter, Member nominee)
        {
            if (!owners.Contains(rejecter))
            {
                throw new ArgumentException($"{rejecter.Username} is not a store owner of store {id}, hence he can not reject to nominate {nominee.Username} as a store owner.");
            }
            if (!owner_voting.TryRemove(nominee, out var value))
            {
                throw new ArgumentException($"{nominee.Username} is not being voted to be a store owner of store {id}, hence you can not reject his nomination.");
            }
        }

        public int GetId()
        {
            return this.id;
        }

        public bool IsOpen()
        {
            return this.open;
        }

        public IReadOnlyDictionary<int, Product> GetProducts()
        {
            return products;
        }

        public String GetStoreName()
        {
            return name;
        }

        public void OpenStore()
        {
            this.open = true;
            storeDAL.Open = true;
            DataHandler.getDBHandler().update(storeDAL);
        }

        public void CloseStore()
        {
            this.open = false;
            storeDAL.Open = false;
            DataHandler.getDBHandler().update(storeDAL);
        }

        public Product AddProduct(string name, int productId, string description, double price, int quantity, string category)
        {
            ValidateName(name);
            ValidatePrice(price);
            ValidateQuantity(quantity);
            ValidateCategory(category);
            Product newProd = new Product(productId, name, description, price, quantity, category, id);
            products.Add(productId, newProd);
            storeDAL.Products.Add(newProd.ToDAL());
            DataHandler.getDBHandler().update(storeDAL);
            return newProd;
        }

        internal void RemoveVotingOnMember(Member nominated)
        {
            owner_voting.TryRemove(nominated, out var voter);
        }

        public void RemoveProduct(int productID)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            products.Remove(productID);
            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            storeDAL.Products.Remove(pDAL);
            DataHandler.getDBHandler().update(storeDAL);
            DataHandler.getDBHandler().remove(pDAL);
        }

        public void ChangeProductName(int productID, string name)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidateName(name);
            products[productID].Name = name;

            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            pDAL.Name = name;
            DataHandler.getDBHandler().update(pDAL);
        }

        public void ChangeProductDescription(int productID, string description)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            products[productID].Description = description;

            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            pDAL.Description = description;
            DataHandler.getDBHandler().update(pDAL);
        }

        public void ChangeProductPrice(int productID, double price)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidatePrice(price);
            products[productID].Price = price;

            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            pDAL.Price = price;
            DataHandler.getDBHandler().update(pDAL);
        }
        public void ChangeProductQuantity(int productID, int quantity)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidateQuantity(quantity);
            products[productID].Quantity = quantity;

            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            pDAL.Quantity = quantity;
            DataHandler.getDBHandler().update(pDAL);
        }

        public void ChangeProductCategory(int productID, string category)
        {
            ValidateID(productID);
            ValidateProductExist(productID);
            ValidateCategory(category);
            products[productID].Category = category;

            ProductDAL pDAL = storeDAL.Products.Find(x => x.Id == productID);
            pDAL.Category = category;
            DataHandler.getDBHandler().update(pDAL);
        }

        public void AddProductDiscount(string json_discount, int product_id)
        {
            if (!products.ContainsKey(product_id))
                throw new Exception("Product ID: " + product_id + " does not exist in store.");
            discountPolicy.AddProductDiscount(json_discount, product_id);
        }

        public void AddCategoryDiscount(string json_discount, string category_name)
        {
            ValidateCategory(category_name);
            discountPolicy.AddCategoryDiscount(json_discount, category_name);
        }

        public void AddStoreDiscount(string json_discount)
        {
            discountPolicy.AddStoreDiscount(json_discount);
        }

        public void AddProductTerm(string json_term, int product_id)
        {
            if (!products.ContainsKey(product_id))
                throw new Exception("Product with ID: " + product_id + " does not exist in store.");
            purchasePolicy.AddProductTerm(json_term, product_id);
        }

        public void AddCategoryTerm(string json_term, string category_name)
        {
            purchasePolicy.AddCategoryTerm(json_term, category_name);
        }

        public void AddStoreTerm(string json_term)
        {
            purchasePolicy.AddStoreTerm(json_term);
        }

        public void AddUserTerm(string json_term)
        {
            purchasePolicy.AddUserTerm(json_term);
        }

        private void ValidateID(int ID)
        {
            if (ID < 0)
                throw new ArgumentOutOfRangeException("Product ID must be bigger than zero.");
        }

        private void ValidateProductExist(int ID)
        {
            if (!products.ContainsKey(ID))
                throw new Exception("Product ID does not exist in the store.");
        }

        private void ValidateName(string name)
        {
            if (name == null || name.Equals(""))
                throw new ArgumentException("Name cannot be empty.");
        }

        private void ValidatePrice(double price)
        {
            if (price < 0)
                throw new ArgumentOutOfRangeException("Price must be bigger than zero.");
        }

        private void ValidateQuantity(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentOutOfRangeException("Quntity must be zero or above.");
        }

        private void ValidateCategory(string category)
        {
            if (category == null || category.Equals(""))
                throw new ArgumentException("Category must be non-empty.");
        }

        public Product GetProduct(int productId)
        {
            if (!products.ContainsKey(productId))
                throw new ArgumentException($"Product with ID {productId} does not exist in the store.");
            return products[productId];
        }

        public ShoppingBagProduct GetProductForSale(int productId, int quantity)
        {
            if (!products.ContainsKey(productId))
                throw new ArgumentException($"Product with ID {productId} does not exist in the store.");
            if (products[productId].Quantity < quantity)
                throw new ArgumentException($"There is not enough quantity from Product with ID {productId}.");
            products[productId].Quantity -= quantity;
            return products[productId].GetShoppingBagProduct(quantity);
        }

        internal void RemoveOwner(Member nominatedMember)
        {
            owners.Remove(nominatedMember);
        }

        internal void restoreProduct(ProductDTO product)
        {
            if(products.ContainsKey(product.Id))
            {
                products[product.Id].Quantity += product.Quantity;
            }
            else
            {
                products.Add(product.Id,new Product(product.Id,product.Name,product.Description,product.Price,product.Quantity, product.Category, id));
            }
        }

        internal void AddToProductQuantity(int productId, int quantity)
        {
            if (products.ContainsKey(productId))
            {
                products[productId].Quantity += quantity;
            }
            else throw new ArgumentException($"Store {id} does not have Product {productId}");
        }

        internal void RemoveFromProductQuantity(int productId, int quantity)
        {
            if (products.ContainsKey(productId))
            {
                if (products[productId].Quantity < quantity)
                {
                    throw new ArgumentException($"Store {name} does not have enough to provide {quantity} of Product {productId}");
                }
                products[productId].Quantity -= quantity;
            }
            else throw new ArgumentException($"Store {id} does not have Product {productId}");
        }

        internal ShoppingBagDTO validateBagInStockAndGet(ShoppingBagDTO shoppingBag)
        {
            
            foreach (ProductDTO product in shoppingBag.products)
            {
                if(products.ContainsKey(product.Id) && products[product.Id].Quantity >= product.Quantity)
                {
                    //products[product.Id].Quantity -= product.Quantity;
                    //todo does it impact thred
                }
                else {
                    throw new ArgumentException($"store {id} doesn't has enough {product.Name} in stock");
                }
                        
            }  
            return shoppingBag;     
        }
        

        internal StoreDTO GetStoreDTO()
        {
            //throw new NotImplementedException();
            return new StoreDTO(id, name, products,open);
        }

        public virtual bool ProductExists(int product_id)
        {
            return products.ContainsKey(product_id);
        }

        internal double CalaculatePrice(ShoppingBagDTO shoppingBag)
        {
            double discount = discountPolicy.CalculateDiscount(shoppingBag);
            double price = 0;
            foreach (ProductDTO productDTO in shoppingBag.products)
            {
                price += productDTO.Price*productDTO.Quantity;
            }
            return Math.Max(price - discount, 0);
        }

        internal void CheckPurchasePolicy(ShoppingBagDTO shoppingBag, int age)
        {
            double price = CalaculatePrice(shoppingBag);
            if (!purchasePolicy.CanPurchase(shoppingBag, age))
                throw new Exception("Cannot purchase shopping bag because it violates our purchase policy.");
        }

        public bool VoteForBid(Member voter, bool vote, int bid_id)
        {
            if (!owners.Contains(voter))
            {
                throw new ArgumentException($"{voter.Username} is not a store owner of store {id}, hence he can not vote on a bid.");
            }
            Bid bid;
            if (!biding_votes.TryGetValue(bid_id, out bid))
            {
                throw new ArgumentException($"{bid_id} is not id of a bid in this store.");
            }
            if(vote)
            {
                return bid.AddOwnerVote(voter) == owners.Count;
            }
            else
            {
                biding_votes.TryRemove(bid_id, out bid);
                return false;
            }
        }

        internal int OfferBid(string username, int storeId, Product product, double price)
        {
            int curr_id = bid_id_count++;
            while (!biding_votes.TryAdd(curr_id, new Bid(curr_id, storeId, product, price, username)))
            {
                curr_id = bid_id_count++;
            }
            return curr_id;
        }

        internal void RemoveBid(int bidId)
        {
            biding_votes.TryRemove(bidId, out var val);
        }

        internal void ChangeBidPrice(int bidId, double newPrice)
        {
            if (biding_votes[bidId].OwnerVotes.Count == owners.Count)
            {
                throw new ArgumentException("Every owner has accepted the bid offer, you can not change the price now!");
            }
            biding_votes[bidId].OfferedPrice = newPrice;
        }

        internal bool CanBuyBid(string requester, int bidId)
        {
            return (biding_votes[bidId].OwnerVotes.Count == owners.Count && biding_votes[bidId].OfferingMembername.Equals(requester)) || biding_votes[bidId].CounterOfferred;
        }
    }
}
