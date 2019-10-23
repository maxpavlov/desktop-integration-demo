using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace backend.Hubs
{
    public class DesktopIntegrationHub : Hub
    {
        public async Task SendMessage(string id, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", id, message);
        }
    }
}