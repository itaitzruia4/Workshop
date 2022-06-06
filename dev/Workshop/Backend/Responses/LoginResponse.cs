using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class LoginResponse: FrontResponse<Member>
    {
        public List<Notification> Notifications { get; set; }

        public LoginResponse(Member member, List<Notification> notifications): base(member)
        {
            Notifications = notifications;
        }

        public LoginResponse(string error): base(error)
        {
            Notifications = new List<Notification>();
        }
    }
}
