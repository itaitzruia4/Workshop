using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Purchases
{
    public class Term
    {
        private static int nextId = 0;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string TermJson { get; set; }

        public Term()
        {
            this.Id = nextId;
            nextId++;
        }

        public Term(string TermJson)
        {
            this.TermJson = TermJson;
            this.Id = nextId;
            nextId++;
        }
    }
}
