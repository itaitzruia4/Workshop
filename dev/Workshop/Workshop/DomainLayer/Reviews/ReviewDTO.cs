using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.Orders;
using Workshop.DomainLayer.UserPackage.Permissions;
using Workshop.DomainLayer.UserPackage.Security;
using Action = Workshop.DomainLayer.UserPackage.Permissions.Action;

namespace Workshop.DomainLayer.Reviews
{
    public class ReviewDTO
    {
        public string Review { get; set; }
        public string Reviewer { get; set; }
        public int ProductId { get; set; }

        public ReviewDTO(string user, int productId, string review)
        {
            this.Reviewer = user;
            this.Review = review;
            this.ProductId = productId;
        }
    }
}