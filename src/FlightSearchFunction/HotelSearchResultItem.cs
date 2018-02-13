using Newtonsoft.Json;

namespace FlightSearchFunction
{
    public class HotelSearchResultItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}