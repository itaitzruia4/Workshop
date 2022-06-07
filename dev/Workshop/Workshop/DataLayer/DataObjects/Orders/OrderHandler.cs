﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DataLayer.DataObjects.Orders
{
    public class OrderHandler<T>: DALObject
    {
        public int Id { get; set; }
        public List<MemberToOrders<T>> MemberToOrders { get; set; }
        public static int CURR_ID { get; set; }

        public OrderHandler()
        {
        }

        public OrderHandler(List<MemberToOrders<T>> memberToOrders)
        {
            MemberToOrders = memberToOrders;
        }
    }
}
