namespace InterviewMauiBlazor.Database.Entities
{
    public class Transaction
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public string Buyer { get; set; }
        public string Seller { get; set; }
        public DateTime Time { get; set; }
        public string Status { get; set; }
    }
}
