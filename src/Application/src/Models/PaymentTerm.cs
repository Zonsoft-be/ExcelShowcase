using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace Application.Models
{
    public class PaymentTerm : Identifiable
    {
        public PaymentTerm()
        {

        }

        public PaymentTerm(string name, int days, string description = null)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name), "Name is required");
            if (days <= 0) throw new ArgumentException(nameof(days), "Days must be greather than 0.");
            if (days > 365) throw new ArgumentException(nameof(days), "Days must be less than 365.");

            this.Name = name;
            this.Description = description;
            this.Days = days;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public int Days { get; set; }

        public DateTime Derive(DateTime date)
        {
            if ("INV".Equals(Name))
            {
                return date.AddDays(this.Days);
            }

            if ("EOM".Equals(Name))
            {
                return date.Date.AddMonths(1).AddDays(this.Days);
            }           

            return date;           
        }

        public override string ToString()
        {
            return $"{this.Name} {this.Description ?? "N/A"} - Days: {this.Days}";
        }
    }
}
