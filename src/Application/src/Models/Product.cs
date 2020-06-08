using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models
{
    public class Product : Identifiable
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal? UnitPrice { get; set; }

        public string Unit { get; set; }

        public decimal? Quantity { get; set; }
    }
}
