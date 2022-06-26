using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Purchases
{
    public class CategoryTerm: DALObject
    {
        public static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public Term Term { get; set; }

        public CategoryTerm()
        {
            this.Id = nextId;
            nextId++;
        }

        public CategoryTerm(string CategoryName, Term Term)
        {
            this.CategoryName = CategoryName;
            this.Term = Term;
            this.Id = nextId;
            nextId++;
        }
    }
}
