using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class EventHub : Hub
    {
        public async Task SendEventUpdate(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveEventUpdate", user, message);
        }
    }
}