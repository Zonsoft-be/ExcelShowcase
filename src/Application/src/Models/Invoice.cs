using Application.Data;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Devices.Enumeration;

namespace Application.Models
{
    public class Invoice : Identifiable
    {
        public const string TagId = "{78B7A83F-F9E2-4336-83BB-14C51B9EF709}";

        public Invoice()
        {
            if(this.InvoiceLines == null)
            {
                this.InvoiceLines = Array.Empty<InvoiceLine>();
            }
        }

        public int InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime InvoiceDueDate { get; set; }

        public Organisation Customer { get; set; }

        public decimal? NetAmount { get; set; }

        public decimal? Tax { get; set; }

        public decimal? Total { get; set; }

        public InvoiceLine[] InvoiceLines { get; set; }

        public void AddInvoiceLine(InvoiceLine invoiceLine)
        {
            if(invoiceLine == null)
            {
                return;
            }

            this.InvoiceLines = this.InvoiceLines.Concat(new[] { invoiceLine }).ToArray();
        }

        public void RemoveInvoiceLine(InvoiceLine invoiceLine)
        {
            if (invoiceLine == null)
            {
                return;
            }
            var index = Array.IndexOf(this.InvoiceLines, invoiceLine);

            this.InvoiceLines =  this.InvoiceLines.Where((item, idx) => idx != index).ToArray();

        }

        public override void OnSave(IDatabase database)
        {
            if(this.InvoiceNumber == 0)
            {
                this.InvoiceNumber = database.Count<Invoice>();
            }

            foreach(var line in this.InvoiceLines)
            {
                line.OnSave(database);
            }

            this.NetAmount = this.InvoiceLines.Sum(l => l.NetAmount);
            this.Tax = this.InvoiceLines.Sum(l => l.Tax);
            this.Total = this.InvoiceLines.Sum(l => l.Total);

            base.OnSave(database);

            // Do stuff
        }

        internal DateTime DeriveDueDate(int days, string scheme)
        {
            if(this.InvoiceDate != null)
            {
                if ("INV".Equals(scheme))
                {
                    this.InvoiceDueDate = this.InvoiceDate.AddDays(days);
                }

                if ("EOM".Equals(scheme))
                {
                    this.InvoiceDueDate = this.InvoiceDate.Date.AddMonths(1).AddDays(days);
                }
            }

            return this.InvoiceDueDate;
        }
    }
}
