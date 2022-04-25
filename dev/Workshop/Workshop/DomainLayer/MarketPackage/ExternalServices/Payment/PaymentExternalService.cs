using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage.ExternalServices.Payment
{
    public class PaymentExternalService : IPaymentExternalService
    {
        public PaymentExternalService() { }

        public bool payAmount(string username, int amount)
        {
            return true;
        }
    }
}
