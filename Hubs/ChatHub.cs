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
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace StepChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MessengerDataDbContext _context;
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
                    var user = await _context.Users.FindAsync(userId);
                    var user2 = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

                    ChatsModel chatsModel = new ChatsModel()
                    {
                        ChatId = _context.Chats.Count() == 0 ? 0 : _context.Chats.Max(x => x.ChatId + 1),
                        User1Name = user.FullName,
                        User2Name = user2.FullName,
                        User1ImageId = user.ImageId,
                        User2ImageId = user2.ImageId,
                        CreateChatUserId = user.Id,
                        Time = DateTime.Now.TimeOfDay
                    };
                    KeysModel keysModel = new KeysModel() { ChatId = chatsModel.ChatId, Key = privateKey };
                    ChatUserModel chatUserModel = new ChatUserModel() { ChatId = chatsModel.ChatId, User1 = user!.Id, User2 = user2.Id };
                    var Image = await _context.Images
                                   .Where(x => x.ImageId == user2.ImageId)
                                   .Select(x => x.Image)
                                   .FirstOrDefaultAsync();

                    await _context.Chats.AddAsync(chatsModel);
                    await _context.Keys.AddAsync(keysModel);
                    await _context.ChatUsers.AddAsync(chatUserModel);
                    await _context.SaveChangesAsync();

                    await Clients.Caller.SendAsync("CreateChat", email, chatsModel.ChatId, user2.FullName, Image);
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
                try
                {
                    var user2 = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

                    var ImageId = await _context.Users
                               .Where(x => x.Id == Id)
                               .Select(x => x.ImageId)
                               .FirstOrDefaultAsync();

                    var chatId = await _context.ChatUsers
                               .Where(x => x.User1 == Id && x.User2 == user2.Id || x.User2 == Id && x.User1 == user2.Id)
                               .Select(x => x.ChatId)
                               .FirstOrDefaultAsync();

                    var chatName = await _context.Users
                               .Where(x => x.Id == Id)
                               .Select(x => x.FullName)
                               .FirstOrDefaultAsync();

                    var image = await _context.Images
                               .Where(x => x.ImageId == ImageId)
                               .Select(x => x.Image)
                               .FirstOrDefaultAsync();

                    MessagesModel messagesModel = new MessagesModel() { UserId = Id, ChatId = chatId, Text = text, CreateTime = DateTime.Now };

                    await _context.AddAsync(messagesModel);
                    await _context.SaveChangesAsync();

                    MessagesStatusModel messagesStatusModel = new MessagesStatusModel() { Id = messagesModel.Id, UserId = Id, IsRead = false };
                    await _context.AddAsync(messagesStatusModel);
                    await _context.SaveChangesAsync();

                    await Clients.User(email!).SendAsync("ReceiveMessage", text, messagesModel.UserId.ToString(), chatId, chatName, image, Context?.User?.Identity?.Name);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public async Task LoadMessages(string? privateKey, int chatId)
        {
            try
            {
                var messages = await _context.Messages.Where(x => x.ChatId == chatId).ToListAsync();

                await Clients.Caller.SendAsync("ReceiveMessage", messages, "0", chatId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}