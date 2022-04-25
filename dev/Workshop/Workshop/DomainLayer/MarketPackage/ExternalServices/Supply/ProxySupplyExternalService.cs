using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage.ExternalServices.Supply
{
    public class ProxySupplyExternalService : ISupplyExternalService
    {
        private ISupplyExternalService externalService;
        public ProxySupplyExternalService(ISupplyExternalService externalService)
        {
            this.externalService = externalService;
        }

        public bool supplyToAddress(string username, string address)
        {
            if(externalService == null)
            {
                return true;
            }
            return true;
        }
    }
}
