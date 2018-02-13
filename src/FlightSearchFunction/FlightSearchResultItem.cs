using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightSearchFunction
{
    public class FlightSearchResultItem
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

    }
}
