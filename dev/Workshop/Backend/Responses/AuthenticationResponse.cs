namespace API.Responses
{
    public class AuthenticationResponse
    {
        public int UserId;
        public string Error;
        public AuthenticationResponse(int userId)
        {
            UserId = userId;
        }
        public AuthenticationResponse(string error)
        {
            Error = error;
        }
    }
}
