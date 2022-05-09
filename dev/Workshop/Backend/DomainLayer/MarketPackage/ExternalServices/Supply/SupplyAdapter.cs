using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Supply;

namespace Workshop.DomainLayer.MarketPackage
{
    public class SupplyAdapter : IMarketSupplyService
    {
        protected ISupplyExternalService externalService;

        public SupplyAdapter(ISupplyExternalService externalService)
        {
            this.externalService = externalService;
        }

        public void supplyToAddress(string username, string address)
        {
            if (!externalService.supplyToAddress(username, address))
            {
                throw new Exception($"cannot supply {username}'s items to address {address}");
            }
        }
    }
}
