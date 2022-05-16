using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainMarketManager = Workshop.DomainLayer.UserPackage.Permissions.MarketManager;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class MarketManager : Role
    {
        public MarketManager(DomainMarketManager dmm) : base(dmm)
        {
        }
    }
}
