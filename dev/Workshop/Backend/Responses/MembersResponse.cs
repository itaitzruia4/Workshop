using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class MembersResponse
    {
        public List<Member> Members { get; set; }
        public string Error { get; set; }

        public MembersResponse(List<Member> members)
        {
            Members = members;
        }
        public MembersResponse(string error)
        {
            Error = error;
        }

    }
}
