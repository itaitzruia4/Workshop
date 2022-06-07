using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.DataLayer.DataObjects;
using Workshop.DataLayer.DataObjects.Members;
using ActionDAL = Workshop.DataLayer.DataObjects.Members.Action;
using Role = Workshop.DataLayer.DataObjects.Members.Role;
namespace Workshop.DataLayer
{
    public class Test
    {
        public static void Main(string[] args)
        {
            Context context = new Context();
            Member member = new Member("pass", "name1", DateTime.Now, new List<Role>(), new DataObjects.Market.ShoppingCart());
            context.Add(member);
            context.SaveChanges();
            //context.Find<Role>(1);
            Console.ReadKey();
        }
    }
}
