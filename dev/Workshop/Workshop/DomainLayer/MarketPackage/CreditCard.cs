namespace Workshop.DomainLayer.MarketPackage
{
    public class CreditCard
    {
        public string Card_number { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Holder { get; set; }
        public string Ccv { get; set; }
        public string Id { get; set; }
        public CreditCard(string card_number, string month, string year, string holder, string ccv, string id)
        {
            Card_number = card_number;
            Month = month;
            Year = year;
            Holder = holder;
            Ccv = ccv;
            Id = id;
        }
    }
}