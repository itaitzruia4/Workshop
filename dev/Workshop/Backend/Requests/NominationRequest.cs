namespace API.Requests
{
    public class NominationRequest
    {
        public int UserId { get; set; }
        public string Nominator { get; set; }
        public string Nominated { get; set; }
        public int StoreId { get; set; }
    }
}
