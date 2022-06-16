using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Reviews
{
    public class UserToReviewDTO : DALObject
    {
        private static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Username { get; set; }
        public ReviewDTO Review { get; set; }

        public UserToReviewDTO()
        {
            this.Id = nextId;
            nextId++;
        }

        public UserToReviewDTO(string username, ReviewDTO review)
        {
            Username = username;
            Review = review;
            this.Id = nextId;
            nextId++;
        }
    }
}
