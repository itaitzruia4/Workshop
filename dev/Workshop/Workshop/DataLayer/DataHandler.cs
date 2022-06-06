using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Workshop.DataLayer.DataObjects.Market;
using System.Threading;

namespace Workshop.DataLayer
{
    public class DataHandler
    {
        Context cache;
        public static DataHandler Instance = null;
        Object lck;

        private DataHandler()
        {
            cache = new Context();
            new Thread(() => upload(20)).Start();
            lck = new Object();
        }

        public static DataHandler getDBHandler()
        {
            if (Instance == null)
                Instance = new DataHandler();
                
            return Instance;
        }
        
        public void save<T>(T toSave) where T : class, DALObject
        {
            lock (lck)
            {
                cache.Add<T>(toSave);
            }
        }

        private void upload(int timeOutInSeconds)
        {
            Context oldContext = null;
            while (true)
            {
                lock (lck)
                {
                    oldContext = cache;
                    cache = new Context();
                }
                oldContext.SaveChanges();
                Thread.Sleep(timeOutInSeconds * 1000);
            }
        }

    }
}
