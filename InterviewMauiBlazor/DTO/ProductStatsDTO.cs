using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewMauiBlazor.DTO
{
    public class ProductStatsDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double TotalQuantity { get; set; }
    }
}
