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
            DataHandler db = DataHandler.getDBHandler();
            db.clear();
            //Console.ReadKey();
        }
    }
}
