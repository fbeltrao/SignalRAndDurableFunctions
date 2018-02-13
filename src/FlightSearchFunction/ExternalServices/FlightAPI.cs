using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlightSearchFunction.ExternalServices
{
    /// <summary>
    /// Flight Search API
    /// </summary>
    public class FlightAPI
    {

        /// <summary>
        /// Searches for a flight
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FlightSearchResultItem>> Search(string origin, string destination, DateTime date)
        {
            var result = new List<FlightSearchResultItem>();
            var rnd = new Random();
            var resultItemCount = rnd.Next(10) + 1;
            for (int i = 0; i < resultItemCount; i++)
            {
                var item = new FlightSearchResultItem()
                {
                    Date = date.Date.AddHours(i + 8),
                    Price = 1000 + (100 * rnd.Next(15)),
                };

                result.Add(item);

                await Task.Delay(100);
            }

            return result;
        }
    }
}
