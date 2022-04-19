using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.ExternalSerivces
{
    interface IPaymentExternalService
    {
        bool ExecuteTransaction(int id, int paymentAmount);
    }
}
