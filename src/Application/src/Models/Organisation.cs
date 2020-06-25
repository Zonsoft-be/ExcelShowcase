using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models
{
    public class Organisation : Identifiable
    {
        public const string TagId = "{344583ED-FAEC-495E-AE45-FAD59F457AF2}";

        public Organisation()
        {            
        }

        public string Lookup { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public  string  Street { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
        
        public string VatNumber { get; set; }
    }
}
