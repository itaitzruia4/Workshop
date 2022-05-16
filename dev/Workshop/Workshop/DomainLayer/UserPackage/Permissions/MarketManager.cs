﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public class MarketManager : Role
    {
        public MarketManager()
        {
            actions.Add(Action.ViewClosedStore);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return obj.GetType() != typeof(MarketManager);
        }
    }
}
