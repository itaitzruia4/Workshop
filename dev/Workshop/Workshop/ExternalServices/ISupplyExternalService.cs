using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.ExternalServices
{
    interface ISupplyExternalService
    {
        bool ExecuteTransaction(int id, int products, string Address);
    }
}
