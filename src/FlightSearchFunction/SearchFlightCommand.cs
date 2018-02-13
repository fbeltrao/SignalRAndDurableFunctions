using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightSearchFunction
{
    public class SearchFlightCommand
    {
        [JsonProperty("searchId")]
        public string SearchId { get; set; }

        [JsonProperty("returnUrl")]
        public string ReturnUrl { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
    }
}
