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
        string Review { get; set; }
        string Reviewer { get; set; }
        int ProductId { get; set; }
        int Grade { get; set; }

        public ReviewDTO(string user, int productId, string review)
        {
            this.Reviewer = user;
            this.Review = review;
            this.ProductId = productId;
            //this.Grade = grade;
        }
    }
}