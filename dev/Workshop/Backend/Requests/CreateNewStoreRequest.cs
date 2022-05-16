namespace API.Requests
{
    public class CreateNewStoreRequest
    {
        public int UserId { get; set; }
        public string Creator { get; set; }
        public string StoreName { get; set; }
    }
}
