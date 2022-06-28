using System.Text;
using WebSocketSharp.Server;
using Workshop.ServiceLayer.ServiceObjects;
using Workshop.ServiceLayer;
using Newtonsoft.Json;

namespace API
{
    public class AdminBehavior : WebSocketBehavior
    {

    }
    public class StaisticsViewingServer : WebSocketServer
    {
        public StaisticsViewingServer(System.Net.IPAddress address, int port) : base(address, port) 
        { 

        }

        public void AddAdminPath(string admin_name)
        {
            string relativeServicePath = "/" + admin_name + "-live_view";
            try
            {
                if (WebSocketServices[relativeServicePath] == null)
                    AddWebSocketService<AdminBehavior>(relativeServicePath);
            }
            catch
            {
                throw new Exception("Sorry, but it seems that we cant connect you");
            }
        }

        public void SendMessageToAllAdmins(Response<StatisticsInformation> si)
        {
            if (!si.ErrorOccured)
            {
                string tosend = JsonConvert.SerializeObject(si.Value);
                foreach (string path in WebSocketServices.Paths)
                {
                    WebSocketServices[path].Sessions.Broadcast(tosend);
                }
            }
        }
    }
}
