using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FlightSearchFunction.ExternalServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlightSearchFunction
{
    public static class FlightSearchOrchestration
    {
        [FunctionName("FlightSearchOrchestration")]
        public static async Task<SearchFlightResult> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {

            var searchFlightCommand = context.GetInput<SearchFlightCommand>();
            
            var searchFlightTask = context.CallActivityAsync<IEnumerable<FlightSearchResultItem>>(nameof(SearchFlightActivity), searchFlightCommand);
            var searchHotelsTask = context.CallActivityAsync<IEnumerable<HotelSearchResultItem>>(nameof(SearchHotelActivity), searchFlightCommand);
            var searchTicketTask = context.CallActivityAsync<IEnumerable<TicketSearchResultItem>>(nameof(SearchTicketActivity), searchFlightCommand);

            var result = new SearchFlightResult()
            {
                SearchId = searchFlightCommand.SearchId,
            };


            // Optionally search for tickets, but do not wait longer than 5 seconds
            using (var timeoutCts = new CancellationTokenSource())
            {                
                var ticketSearchTimeoutTask = context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(10), timeoutCts.Token);

                await Task.WhenAll(new Task[] { searchFlightTask, searchHotelsTask });
           
                var winner = await Task.WhenAny(searchTicketTask, ticketSearchTimeoutTask);                
                if (winner == searchTicketTask)
                {
                    result.Tickets = searchTicketTask.Result;
                }

                if (!ticketSearchTimeoutTask.IsCompleted)
                    timeoutCts.Cancel();
            }

            result.Flights = searchFlightTask.Result;
            result.Hotels = searchHotelsTask.Result;
            result.SearchTime = (int)DateTime.UtcNow.Subtract(context.CurrentUtcDateTime).TotalMilliseconds;


            if (!string.IsNullOrEmpty(searchFlightCommand.ReturnUrl))
            {
                var callReturnActivityCommand = new CallReturnActivityCommand()
                {
                    ReturnUrl = searchFlightCommand.ReturnUrl,
                    Result = result
                };

                await context.CallActivityAsync<bool>(nameof(CallReturnUrlActivity), callReturnActivityCommand);
            }

            return result;
        }

        static HttpClient httpClient = new HttpClient();
        [FunctionName(nameof(CallReturnUrlActivity))]

        public static async Task<bool> CallReturnUrlActivity([ActivityTrigger] CallReturnActivityCommand callReturnActivityCommand, TraceWriter log)
        {
            try
            {               
                var response = await HttpClientFactory.Create().PostAsJsonAsync(callReturnActivityCommand.ReturnUrl, callReturnActivityCommand.Result);
                if (response.IsSuccessStatusCode)
                    return true;
            }
            catch (Exception ex)
            {
                log.Error("Calling return url", ex);
            }
            
            return false;
                
        }

        [FunctionName(nameof(SearchTicketActivity))]
        public static async Task<IEnumerable<TicketSearchResultItem>> SearchTicketActivity([ActivityTrigger] SearchFlightCommand searchFlightCommand, TraceWriter log)
        {
            log.Info($"Searching for tickets");
            return await new TicketAPI().Search(searchFlightCommand.Destination, searchFlightCommand.StartDate);
           
        }

        [FunctionName(nameof(SearchHotelActivity))]
        public static async Task<IEnumerable<HotelSearchResultItem>> SearchHotelActivity([ActivityTrigger] SearchFlightCommand searchFlightCommand, TraceWriter log)
        {
            log.Info($"Searching for hotels");

            return await new HotelAPI().Search(searchFlightCommand.Destination, searchFlightCommand.StartDate);
        }

        [FunctionName(nameof(SearchFlightActivity))]
        public static async Task<IEnumerable<FlightSearchResultItem>> SearchFlightActivity([ActivityTrigger] SearchFlightCommand searchFlightCommand, TraceWriter log)
        {
            log.Info($"Searching for flights");
            return await new FlightAPI().Search(searchFlightCommand.Origin, searchFlightCommand.Destination, searchFlightCommand.StartDate);
            
        }

        [FunctionName("FlightSearchOrchestration_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            TraceWriter log)
        {

            //req.Properties[""]
            // Function input comes from the request content.
            var httpContext = (HttpContext)req.Properties[nameof(HttpContext)];
            var input = new SearchFlightCommand()
            {
                SearchId = httpContext.Request.Query["searchId"].ToString(),
                ReturnUrl = httpContext.Request.Query["returnUrl"].ToString(),
                Destination = httpContext.Request.Query["destination"].ToString(),
                Origin = httpContext.Request.Query["origin"].ToString(),
                StartDate = DateTime.Parse(httpContext.Request.Query["startDate"].ToString()),
            };

            string instanceId = await starter.StartNewAsync("FlightSearchOrchestration", input);

            log.Info($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}