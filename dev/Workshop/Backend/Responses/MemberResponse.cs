using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class MemberResponse
    {
        public Member Member { get; set; }
        public string Error { get; set; }

        public MemberResponse(Member m)
        {
            Member = m;
        }
        public MemberResponse(string error)
        {
            Error = error;
        }

    }
}
