using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class UserToReviewDTO : DALObject
    {
        public string Username { get; set; }
        public ReviewDTO Review { get; set; }

        public UserToReviewDTO()
        {
        }

        public UserToReviewDTO(string username, ReviewDTO review)
        {
            Username = username;
            Review = review;
        }
    }
}
