using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Purchases
{
    public class ProductTerm: DALObject
    {
        public static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Term Term { get; set; }

        public ProductTerm()
        {
            this.Id = nextId;
            nextId++;
        }

        public ProductTerm(int ProductId, Term Term)
        {
            this.ProductId = ProductId;
            this.Term = Term;
            this.Id = nextId;
            nextId++;
        }
    }
}
