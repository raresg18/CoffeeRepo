using System;
using System.Collections.Generic;
using System.Text;

namespace CafePOS
{
    internal class SaleRecord
    {
            public DateTime Date { get; set; }
            public string ItemName { get; set; }
            public int Quantity { get; set; }
            public double Total { get; set; }
  
    }
}
