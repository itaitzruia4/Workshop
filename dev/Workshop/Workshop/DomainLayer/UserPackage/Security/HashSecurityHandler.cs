using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Security
{
    class HashSecurityHandler : ISecurityHandler
    {
        public string Encrypt(string message)
        {
            return message.GetHashCode().ToString();
        }
    }
}
