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
        private Context cache;
        private static DataHandler Instance = null;


        private DataHandler()
        {
            cache = new Context();
            new Thread(() => upload(20)).Start();
        }

        public static DataHandler getDBHandler()
        {
            if (Instance == null)
                Instance = new DataHandler();
                
            return Instance;
        }
        
        public void save<T>(T toSave) where T : class, DALObject
        {
            cache.Add(toSave);
        }

        public void update<T>(T toUpdate) where T : class, DALObject
        {
            cache.Update(toUpdate);
        }

        public void remove<T>(T toRemove) where T : class, DALObject
        {
            cache.Remove(toRemove);
        }

        private void upload(int timeOutInSeconds)
        {
            while (true)
            {
                Thread.Sleep(timeOutInSeconds * 1000);
                cache.SaveChanges();
            }
        }

    }
}
