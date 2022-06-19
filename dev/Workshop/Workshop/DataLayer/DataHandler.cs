using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Workshop.DataLayer.DataObjects.Market;
using System.Threading;
using Workshop.DomainLayer.Loggers;

namespace Workshop.DataLayer
{
    public class DataHandler
    {
        private Context cache;
        private static DataHandler Instance = null;


        private DataHandler()
        {
            cache = new Context();
            //cache.ChangeTracker.AutoDetectChangesEnabled = false;
            //new Thread(() => upload(2)).Start();
        }

        public static DataHandler getDBHandler()
        {
            if (Instance == null)
                Instance = new DataHandler();
                
            return Instance;
        }
        
        public T save<T>(T toSave) where T : class, DALObject
        {
            T output = cache.Add(toSave).Entity;
            cache.SaveChanges();
            return output;
        }

        public void update<T>(T toUpdate) where T : class, DALObject
        {
            cache.Update(toUpdate);
        }

        public void remove<T>(T toRemove) where T : class, DALObject
        {
            cache.Remove(toRemove);
        }

        public T find<T>(Type entityType, object key) where T : class, DALObject
        {
            return (T)cache.Find(entityType, key);
        }

        private void upload(double timeOutInSeconds)
        {
            while (true)
            {
                Logger.Instance.LogEvent($"Saving in {timeOutInSeconds} secs to the DB");
                Thread.Sleep((int)timeOutInSeconds * 1000);
                Logger.Instance.LogEvent($"Trying to upload data to DB");
                try
                {
                    cache.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogEvent($"Exception!!!!!!!!!!!!!!!! " + ex.Message + ex.InnerException);
                }
                Logger.Instance.LogEvent($"Upload succeded");

            }
        }

    }
}
