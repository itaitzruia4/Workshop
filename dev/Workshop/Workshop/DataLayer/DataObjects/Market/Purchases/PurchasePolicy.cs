using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Purchases
{
    public class PurchasePolicy : DALObject
    {
        private static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public List<Term> products_terms { get; set; }
        public List<Term> category_terms { get; set; }
        public Term user_terms { get; set; }
        public Term store_terms { get; set; }

        public PurchasePolicy()
        {
            this.Id = nextId;
            nextId++;
        }

        public PurchasePolicy(List<Term> products_terms, List<Term> category_terms, Term user_terms, Term store_terms)
        {
            this.products_terms = products_terms;
            this.category_terms = category_terms;
            this.user_terms = user_terms;
            this.store_terms = store_terms;
            this.Id = nextId;
            nextId++;
        }
    }
}
