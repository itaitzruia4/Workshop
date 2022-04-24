using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DomainLayer.MarketPackage.ExternalServices.Payment;

namespace Workshop.DomainLayer.MarketPackage
{
    public class PaymentAdapter : IMarketPaymentService
    {
        protected IPaymentExternalService externalService;

        public PaymentAdapter(IPaymentExternalService externalService) 
        {
            this.externalService = externalService;
        }

        public void PayAmount(string username, int amount)
        {
            if(!externalService.PayAmount(username, amount))
            {
                throw new Exception($"user {username} cannot pay ${amount}");
            }
        }
    }
}
