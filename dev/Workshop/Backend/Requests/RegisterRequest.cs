namespace API.Requests
{
    public class RegisterRequest
    {
        public int UserId { get; set; }
        public string Membername { get; set; }
        public string Password { get; set; }
        public string Birthdate { get; set; }
    }
}
