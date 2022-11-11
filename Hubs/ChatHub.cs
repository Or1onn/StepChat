using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using StepChat.Classes;
using StepChat.Models;
using System.Security.Claims;

namespace StepChat.Hubs
{
    public class ChatHub : Hub
    {
        [Authorize]
        public async Task Send(string message, string userId)
        {
            var _userId = Context.UserIdentifier;

            await Clients.User(userId).SendAsync("Receive", userId, message);
        }
    }
}
