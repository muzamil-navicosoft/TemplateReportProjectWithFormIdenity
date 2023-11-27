using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public bool IsPaid { get; set; }
        public string Month { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }

        public Nullable<System.DateTime> PaidDate { get; set; }
  
    }
}