using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Market.Purchases
{
    public class Term
    {
        public int Id { get; set; }
        public string TermJson { get; set; }

        public Term()
        {
        }

        public Term(string TermJson)
        {
            this.TermJson = TermJson;
        }
    }
}
