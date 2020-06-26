using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;

namespace Application.Models
{
    public class Organisation : Identifiable
    {
        public Organisation()
        {            
        }

        public string Name { get; set; }
        
        public  string  Street { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
        
        public string VatNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string FinancialContact { get; set; }
    }
}
