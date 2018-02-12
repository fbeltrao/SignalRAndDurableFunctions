using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightSearchWeb.Controllers
{
    [Route("api/[controller]")]
    public class FlightDataController : Controller
    {
        private readonly string flightApiUrl;
        private readonly IHubContext<WebsiteHub> websiteHub;

        public FlightDataController(IHubContext<WebsiteHub> websiteHub, IConfiguration configuration)
        {
            this.flightApiUrl = configuration["flightAPI:url"] ?? "http://localhost:7071/api/FlightSearchOrchestration_HttpStart";
            this.websiteHub = websiteHub;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Search(string searchId, string origin, string destination, DateTime? startDate, string connectionId)
        {            
            HttpClient httpClient = new HttpClient();
            
            string returnUrl  = string.Format(
                "{0}://{1}{2}",
                Request.IsHttps ? "https" : "http",
                Request.Host,
                Url.Action(nameof(SearchFinished), new { connectionId }));
            
            var uri = $"{flightApiUrl}?searchId={searchId}&origin={origin}&destination={destination}&startDate={startDate.Value.ToString()}&returnUrl={returnUrl}";
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await httpClient.SendAsync(httpRequestMessage);

            await websiteHub.Clients.AllExcept(new string[] { connectionId }).InvokeAsync("Broadcast", $"Someone is searching for tickets from {origin} to {destination}");

            return Ok("Search started");
        }

        /// <summary>
        /// Notified when a search has been finished
        /// Use SignalR to notify connection
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SearchFinished(string connectionId)
        {            
            var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

            await this.websiteHub.Clients.Client(connectionId).InvokeAsync("SearchResult", payload);

            return Ok();
        }


    }
}
