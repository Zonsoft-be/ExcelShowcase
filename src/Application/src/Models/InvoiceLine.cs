using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models
{
    public class InvoiceLine : Identifiable
    {
        public string ProductName { get; set; }

        public decimal? UnitPrice { get; set; }

        public string Unit { get; set; }

        public decimal? Quantity { get; set; }
    }
}
