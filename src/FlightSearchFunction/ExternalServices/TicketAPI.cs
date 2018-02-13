using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlightSearchFunction.ExternalServices
{
    /// <summary>
    /// Ticket search API
    /// </summary>
    public class TicketAPI
    {
        /// <summary>
        /// Searches for tickets
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TicketSearchResultItem>> Search(string destination, DateTime startDate)
        {
            var result = new List<TicketSearchResultItem>();
            var rnd = new Random();
            var resultItemCount = rnd.Next(2);           
            for (int i = 0; i < resultItemCount; i++)
            {
                var item = new TicketSearchResultItem()
                {
                    Name = $"Ticket {i + 1}",
                    Price = 100 + (10 * rnd.Next(15)),
                };

                result.Add(item);
            }

            var delayTime = rnd.Next(50) * 100;
            await Task.Delay(delayTime);

            return result;
        }
    }
}
