using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects
{
    internal class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }

        /* Will be Stored as the dec value of the binary representation
         * 1 if Actions is available and 0 if not
         */
        public int Actions { get; set; }

        public override string ToString()
        {
            return $"{Id} {RoleName} {Actions}";
        }
    }
}
