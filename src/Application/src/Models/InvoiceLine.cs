using Application.Data;
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
            // parameterless constructor for serialization
        }

        public InvoiceLine(int index)
        {
            this.TaxRate = 0.21M;
            this.Index = index;
        }

        public int Index { get; set; }

        public string Description { get; set; }

        public decimal? TaxRate { get; set; }

        public decimal? UnitPrice { get; set; }

        public string Unit { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? NetAmount => this.UnitPrice * this.Quantity;

        public decimal? Tax => this.NetAmount * this.TaxRate;

        public decimal? Total => new decimal?[] { this.NetAmount, this.Tax }.Sum();

        public override void OnSave(IDatabase database)
        { 
        }
    }
}
