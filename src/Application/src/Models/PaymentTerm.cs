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

        public PaymentTerm(string name, int days, bool endOfMonth, string description = null)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name), "Name is required");
            if (days <= 0) throw new ArgumentException(nameof(days), "Days must be greather than 0.");
            if (days > 365) throw new ArgumentException(nameof(days), "Days must be less than 365.");

            this.Name = name;
            this.Days = days;
            this.EndOfMonth = endOfMonth;
            this.Description = description;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Days { get; set; }

        /// <summary>
        /// Gets or sets EndOfMonth. when true the # Days start at the end of given date.
        /// </summary>
        public bool EndOfMonth { get; set; }

        public DateTime Derive(DateTime date)
        {
            if (this.EndOfMonth)
            {
                var dateEndOfMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);

                return dateEndOfMonth.AddDays(this.Days);
            }

            return date.AddDays(this.Days);
        }

        public override string ToString()
        {
            return $"{this.Name}. Days: {this.Days} EOM:{this.EndOfMonth}";
        }
    }
}
