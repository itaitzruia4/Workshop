using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage.ExternalServices.Payment
{
    public interface IPaymentExternalService
    {

        bool PayAmount(string username, int amount);
    }
}
