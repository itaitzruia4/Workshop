namespace API.Responses
{
    public class AuthenticationResponse
    {
        public int UserId { get; set; }
        public string Error { get; set; }
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
