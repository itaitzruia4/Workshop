using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage.ExternalServices.Payment
{
    public interface IMarketPaymentService
    {
        void PayAmount(string username, double amount);
    }
}
