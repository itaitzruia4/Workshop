using Workshop.ServiceLayer.ServiceObjects;

namespace API.Responses
{
    public class MemberResponse
    {
        public Member Member;
        public string Error;

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
