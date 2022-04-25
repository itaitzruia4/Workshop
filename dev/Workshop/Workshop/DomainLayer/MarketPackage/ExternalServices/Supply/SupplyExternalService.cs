using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage.ExternalServices.Supply
{
    public class SupplyExternalService: ISupplyExternalService
    {
        public SupplyExternalService() { }

        public bool supplyToAddress(string username, string address)
        {
            return true;
        }
    }
}
