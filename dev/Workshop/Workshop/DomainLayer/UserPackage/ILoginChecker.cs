using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Workshop.DomainLayer.UserPackage;

namespace Workshop.DomainLayer
{
    public interface ILoginChecker
    {
        bool CheckOnlineStatus(string user);
    }
}
