using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.Loggers
{
    class EventLogger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public EventLogger()
        {

        }
        static void Main(string[] args)
        {
            log.Info("Testing!");
            Console.ReadLine();
        }
    }
}
