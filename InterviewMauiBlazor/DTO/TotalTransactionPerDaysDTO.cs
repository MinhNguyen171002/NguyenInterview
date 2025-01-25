using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewMauiBlazor.DTO
{
    public class TotalTransactionPerDaysDTO
    {
        public double TotalTransaction { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime date { get; set; }
    }
}
