using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NuGet.Configuration;
using StepChat.Classes;
using StepChat.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StepChat.Hubs
{
    public class ChatHub : Hub
    {
        [Authorize]
        public async Task Send(MessagesModel? context, string? userId)
        {
            await Clients.User(userId!).SendAsync("Receive", context);
        }
    }
}
