using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.ServiceLayer
{
    public interface IExternalSystem
    {
        bool IsExternalSystemOnline();
        int Pay(string card_number, string month, string year, string holder, string ccv, string id);
        int Cancel_Pay(int transaction_id);
        int Supply(string name, string address, string city, string country, string zip);
        int Cancel_Supply(int transaction_id);
    }
}
