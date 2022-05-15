namespace API.Requests
{
    public class LoginRequest
    {
        public int UserId { get; set; }
        public string Membername { get; set; }
        public string Password { get; set; }
    }
}
