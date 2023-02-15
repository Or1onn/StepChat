using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using StepChat.Contexts;
using StepChat.Models;
using System.Security.Cryptography;
using System.Security.Policy;

namespace StepChat.Hubs
{
    public class ChatHub : Hub
    {
        MessengerDataDbContext? _context;
        public ChatHub(MessengerDataDbContext? context)
        {
            _context = context;
        }

        public async Task StartMessaging(string? email, string? privateKey, int userId)
        {
            if (email != null && privateKey != null)
            {
                try
                {
                    var user = await _context!.Users.FindAsync(userId);
                    var user2 = await _context!.Users.FirstOrDefaultAsync(x => x.Email == email);

                    ChatsModel chatsModel = new ChatsModel() { ChatId = _context.Chats.Count() == 0 ? 0 : _context.Chats.Max(x => x.ChatId + 1), Name = user2.FullName, ImageId = user2.ImageId, Time = DateTime.Now.TimeOfDay };
                    KeysModel keysModel = new KeysModel() { ChatId = chatsModel.ChatId, Key = privateKey };
                    ChatUserModel chatUserModel = new ChatUserModel() { ChatId = chatsModel.ChatId, User1 = user!.Id, User2 = user2.Id };

                    await _context.Chats.AddAsync(chatsModel);
                    await _context.Keys.AddAsync(keysModel);
                    await _context.ChatUsers.AddAsync(chatUserModel);

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }
        public async Task SendMessage(string? text, string? email, int Id)
        {
            if (email != null && text != null)
            {
                var user2 = await _context!.Users
                           .Where(e => e.Email == email)
                           .Select(e => e.Id)
                           .FirstOrDefaultAsync();

                var chatId = await _context!.ChatUsers
                           .Where(x => x.User1 == Id && x.User2 == user2)
                           .Select(e => e.ChatId)
                           .FirstOrDefaultAsync();

                MessagesModel messagesModel = new MessagesModel() { UserId = Id, ChatId = chatId, Text = text, CreateTime = DateTime.Now };

                await _context.AddAsync(messagesModel);
                await _context.SaveChangesAsync();

                await Clients.User(email!).SendAsync("ReceiveMessage", text);
            }
        }

        public async Task LoadMessages(string? privateKey, int chatId)
        {
            try
            {
                var userEmail = Context?.User?.Identity?.Name;

                var messages = await _context!.Messages.Where(x => x.ChatId == chatId).ToListAsync();

                await Clients.User(userEmail!).SendAsync("ReceiveMessage", messages, privateKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
