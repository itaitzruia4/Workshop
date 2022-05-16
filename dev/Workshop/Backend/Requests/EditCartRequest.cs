namespace API.Requests
{
    public class EditCartRequest
    {
        public int UserId { get; set; }
        public string Membername { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
