using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class UserToReviewDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public ReviewDTO Review { get; set; }

        public UserToReviewDTO()
        {
        }

        public UserToReviewDTO(int id, string username, ReviewDTO review)
        {
            Id = id;
            Username = username;
            Review = review;
        }
    }
}
