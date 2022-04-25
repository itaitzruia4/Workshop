using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage.ExternalServices.Payment
{
    public class ProxyPaymentExternalService : IPaymentExternalService
    {
        private IPaymentExternalService externalService;
        public ProxyPaymentExternalService(IPaymentExternalService externalService)
        {
            this.externalService = externalService;
        }

        public bool PayAmount(string username, double amount)
        {
            if(externalService == null)
            {
                return true;
            }
            return true;
        }
    }
}
