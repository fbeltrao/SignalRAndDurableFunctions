using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightSearchFunction
{
    public class SearchFlightResult
    {
        [JsonProperty("flights")]
        public IEnumerable<FlightSearchResultItem> Flights { get; set; }

        [JsonProperty("hotels")]
        public IEnumerable<HotelSearchResultItem> Hotels { get; set; }

        /// <summary>
        /// Amount of milliseconds needed to retrieve results
        /// </summary>
        [JsonProperty("searchTime")]
        public int SearchTime { get; set; }

        [JsonProperty("tickets")]
        public IEnumerable<TicketSearchResultItem> Tickets { get; set; }

        [JsonProperty("searchId")]
        public string SearchId { get; set; }
    }
}
