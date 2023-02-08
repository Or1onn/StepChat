using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using StepChat.Contexts;
using StepChat.Models;
using System.Security.Cryptography;

namespace StepChat.Hubs
{
    public class ChatHub : Hub
    {
        MessengerDataDbContext? _context;
        public ChatHub(MessengerDataDbContext? context)
        {
            _context = context;
        }

        public async Task StartMessaging(string? userId, string? privateKey)
        {
            if (userId != null && privateKey != null)
            {
                try
                {
                    KeysModel keysModel = new();
                    string? _tmp = null;

                    var userEmail = Context?.User?.Identity?.Name;
                    if (userEmail != null)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            var user = await _context!.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                            keysModel.KeyOwnerId = user!.PrivateKeysStorageId;
                            keysModel.Email = userId;
                            keysModel.Key = privateKey;

                            await _context.Keys.AddAsync(keysModel);

                            await _context.SaveChangesAsync();

                            if (String.IsNullOrEmpty(_tmp))
                            {
                                _tmp = userEmail;
                                userEmail = userId;
                                userId = _tmp;

                                keysModel = new();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }
        public async Task SendMessage(MessagesContext? context, string? userId)
        {
            if (userId != null && context != null)
            {
                await Clients.User(userId!).SendAsync("ReceiveMessage", context);
            }
        }

    }
}
