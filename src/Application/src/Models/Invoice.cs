using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models
{
    public class Invoice : Identifiable
    {
        public int InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public Customer Customer { get; set; }

        public IEnumerable<Invoice> InvoiceLines { get; set; }
    }
}
