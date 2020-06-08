using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models
{
    public class Customer : Identifiable
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public  string  Street { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
        
        public string VatNumber { get; set; }
    }
}
