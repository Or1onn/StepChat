using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NuGet.Packaging.Signing;
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
                    string? _tmp = null;

                    var userEmail = Context?.User?.Identity?.Name;
                    if (userEmail != null)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            var user = await _context!.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                            KeysModel keysModel = new KeysModel() { Email = userId, KeyOwnerId = user!.PrivateKeysStorageId, Key = privateKey };
                            ChatsModel chatsModel = new ChatsModel() { ChatId = _context.Chats.Count() == 0 ? 0 : _context.Chats.Max(x => x.ChatId + 1), Name = user.FullName, CreateChatUserId = user.Id };
                            ChatUserModel chatUserModel = new ChatUserModel() { ChatId = chatsModel.Id, UserId = user.Id };

                            await _context.Keys.AddAsync(keysModel);
                            await _context.Chats.AddAsync(chatsModel);
                            await _context.ChatUsers.AddAsync(chatUserModel);

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
        public async Task SendMessage(string? text, string? userId)
        {
            if (userId != null && text != null)
            {
                var userEmail = Context?.User?.Identity?.Name;

                var user = await _context!.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
                var chat = await _context!.Chats.FirstOrDefaultAsync(x => x.CreateChatUserId == user!.Id);

                MessagesModel messagesModel = new MessagesModel() { UserId = user!.Id, ChatId = chat!.Id, Text = text, CreateTime = DateTime.Now };

                await _context.AddAsync(messagesModel);
                await _context.SaveChangesAsync();

                await Clients.User(userId!).SendAsync("ReceiveMessage", text);
            }
        }

        public async Task LoadMessages(object? json)
        {
            if (json != null)
            {
                try
                {
                    var userEmail = Context?.User?.Identity?.Name;
                    dynamic data = JObject.Parse(json!.ToString());

                    int chatId = data["chatId"];
                    string key = data["key"];

                    var messages = await _context!.Messages.Where(x => x.ChatId == chatId).ToListAsync();

                    foreach (var item in messages)
                    {

                        await Clients.User(userEmail!).SendAsync("ReceiveMessage", item.Text, key);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
