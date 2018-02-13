using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightSearchFunction
{
    public class TicketSearchResultItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
