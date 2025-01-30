using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewMauiBlazor.DTO
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public int orderId { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal TotalPrice { get; set; }

        public string Buyer { get; set; }
        public string Seller { get; set; }
        public DateTime Time { get; set; }
        public string Status { get; set; }

        public ProductDTO Product { get; set; }
        public OrderDTO Order { get; set; }
    }
}
