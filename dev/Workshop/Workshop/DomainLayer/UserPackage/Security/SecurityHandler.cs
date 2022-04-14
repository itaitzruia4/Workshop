using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Security
{
    class HashCodeSecurityHandler : ISecurityHandler
    {
        public string EncryptPassword(string password)
        {
            throw new NotImplementedException();
        }

        public bool IsSamePassword(string passwordCandidate, string encryptedPassword)
        {
            throw new NotImplementedException();
        }
    }
}
