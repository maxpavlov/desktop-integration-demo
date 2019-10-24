using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace companion.Services
{
    public class SignalRService : ISignalRService
    {
        private HubConnection connection;
        private string url = "http://localhost:55344/hub";

        public event Action<string, string> NewTextMessage;

        public void Connect()
        {
            try
            {
                connection = new HubConnectionBuilder().WithUrl(url).Build();
                connection.On<string, string>("SendMessage", (n, m) => NewTextMessage?.Invoke(n, m));

                ServicePointManager.DefaultConnectionLimit = 10;

                connection.StartAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SendMessage(string id, string msg)
        {
            connection.InvokeAsync("SendMessage", id, msg);
        }
    }
}
