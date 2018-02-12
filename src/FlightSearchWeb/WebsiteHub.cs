using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightSearchWeb
{
    public class WebsiteHub : Hub
    {
        public async Task Broadcast(string message)
        {
            await Clients.All.InvokeAsync("Send", message);
        }

        public async Task PrivateMessage(string connectionId, string message)
        {
            await Clients.Client(connectionId)?.InvokeAsync("Send", message);
        }
    }
}
