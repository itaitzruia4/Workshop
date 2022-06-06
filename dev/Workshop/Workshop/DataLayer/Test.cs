using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects;
using Workshop.DataLayer.DataObjects.Members;
using ActionDAL = Workshop.DataLayer.DataObjects.Members.Action;

namespace Workshop.DataLayer
{
    public class Test
    {
        public static void Main(string[] args)
        {
            Context context = new Context();
            Role role = new Role(1, new List<ActionDAL>(), "");
            context.Add(role);
            context.SaveChanges();
            role.StoreId = 2;
            context.Update(role);
            context.SaveChanges();
            //context.Find<Role>(1);
            Console.ReadKey();
        }
    }
}
