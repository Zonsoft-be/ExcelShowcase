using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Models
{
    public class InvoiceLine : Identifiable
    {
        public InvoiceLine()
        {
            this.TaxRate = 0.21M;            
        }

        public int Index { get; set; }

        public string ProductName { get; set; }

        public decimal? TaxRate { get; set; }

        public decimal? UnitPrice { get; set; }

        public string Unit { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? NetAmount => this.UnitPrice * this.Quantity;

        public decimal? Tax => this.NetAmount * this.TaxRate;

        public decimal? Total => new decimal?[] { this.NetAmount, this.Tax }.Sum(); 
    }
}
