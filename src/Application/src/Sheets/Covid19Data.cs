using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Sheets
{  

    public class Covid19Data
    {
        public Datum[] data { get; set; }
        public bool _cacheHit { get; set; }
    }

    public class Datum
    {
        public Coordinates coordinates { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int? population { get; set; }
        public DateTime updated_at { get; set; }
        public Today today { get; set; }
        public Latest_Data latest_data { get; set; }
    }

    public class Coordinates
    {
        public float? latitude { get; set; }
        public float? longitude { get; set; }
    }

    public class Today
    {
        public int deaths { get; set; }
        public int confirmed { get; set; }
    }

    public class Latest_Data
    {
        public int deaths { get; set; }
        public int confirmed { get; set; }
        public int recovered { get; set; }
        public int critical { get; set; }
        public Calculated calculated { get; set; }
    }

    public class Calculated
    {
        public float? death_rate { get; set; }
        public float? recovery_rate { get; set; }
        public object recovered_vs_death_ratio { get; set; }
        public float cases_per_million_population { get; set; }
    }

}