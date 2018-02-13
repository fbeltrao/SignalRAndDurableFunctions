using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlightSearchFunction.ExternalServices
{
    public class HotelAPI
    {
        /// <summary>
        /// Searchs for hotels
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<HotelSearchResultItem>> Search(string city, DateTime date)
        {
            var result = new List<HotelSearchResultItem>();
            var rnd = new Random();
            var resultItemCount = rnd.Next(5) + 1;
            for (int i = 0; i < resultItemCount; i++)
            {
                var item = new HotelSearchResultItem()
                {
                    Name = $"Hotel {i + 1}",
                    Price = 1000 + (100 * rnd.Next(15)),
                };

                result.Add(item);


                await Task.Delay(100);
            }

            return result;
        }
    }
}
