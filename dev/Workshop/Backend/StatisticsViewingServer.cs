using WebSocketSharp.Server;

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
    }
}
