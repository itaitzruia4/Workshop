﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.MarketPackage.ExternalServices.Supply
{
    public interface IMarketSupplyService
    {
        void supplyToAddress(string username, string address);
    }
}