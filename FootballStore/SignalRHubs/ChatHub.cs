using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace FootballStore.SignalRHubs
{
    public class ChatHub : Hub
    {
        public void SendMessageToServer(string userName, string message, string time)
        {
            Clients.Others.sendMessageToClients(userName, message, time);
        }
    }
}