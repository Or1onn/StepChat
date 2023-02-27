using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StepChat.Contexts;
using StepChat.Models;

namespace StepChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MessengerDataDbContext? _context;
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
                    if (_context != null)
                    {
                        var user = await _context.Users.FindAsync(userId);
                        var user2 = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

                        if (user != null)
                        {
                            if (user2 != null)
                            {
                                var chatsModel = new ChatsModel()
                                {
                                    ChatId = !_context.Chats.Any() ? 0 : _context.Chats.Max(x => x.ChatId + 1),
                                    User1Name = user.FullName,
                                    User2Name = user2.FullName,
                                    User1ImageId = user.ImageId,
                                    User2ImageId = user2.ImageId,
                                    CreateChatUserId = user.Id,
                                    Time = DateTime.Now.TimeOfDay
                                };
                                var keysModel = new KeysModel() { ChatId = chatsModel.ChatId, Key = privateKey };
                                var chatUserModel = new ChatUserModel() { ChatId = chatsModel.ChatId, User1 = user.Id, User2 = user2.Id };
                                var image = await _context.Images
                                    .Where(x => x.ImageId == user2.ImageId)
                                    .Select(x => x.Image)
                                    .FirstOrDefaultAsync();

                                await _context.Chats.AddAsync(chatsModel);
                                await _context.Keys.AddAsync(keysModel);
                                await _context.ChatUsers.AddAsync(chatUserModel);
                                await _context.SaveChangesAsync();

                                await Clients.Caller.SendAsync("CreateChat", email, chatsModel.ChatId, user2.FullName, image);
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

        public async Task SendMessage(string text, string email, int id)
        {
            try
            {
                var user2 = await _context?.Users.FirstOrDefaultAsync(x => x.Email == email)!;

                var imageId = await _context.Users
                           .Where(x => x.Id == id)
                           .Select(x => x.ImageId)
                           .FirstOrDefaultAsync();

                var chatId = await _context.ChatUsers
                           .Where(x => user2 != null && (x.User1 == id && x.User2 == user2.Id || x.User2 == id && x.User1 == user2.Id))
                           .Select(x => x.ChatId)
                           .FirstOrDefaultAsync();

                var chatName = await _context.Users
                           .Where(x => x.Id == id)
                           .Select(x => x.FullName)
                           .FirstOrDefaultAsync();

                var image = await _context.Images
                           .Where(x => x.ImageId == imageId)
                           .Select(x => x.Image)
                           .FirstOrDefaultAsync();

                var messagesModel = new MessagesModel() { UserId = id, ChatId = chatId, Text = text, CreateTime = DateTime.Now };

                await _context.AddAsync(messagesModel);
                await _context.SaveChangesAsync();

                var messagesStatusModel = new MessagesStatusModel() { Id = messagesModel.Id, UserId = id, IsRead = false };
                await _context.AddAsync(messagesStatusModel);
                await _context.SaveChangesAsync();

                await Clients.User(email).SendAsync("ReceiveMessage", text, messagesModel.UserId.ToString(), chatId, chatName, image, Context.User?.Identity?.Name);
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }
        }

        public async Task SendFiles(int fileId, string email)
        {
            try
            {
                await Clients.User(email).SendAsync("ReceiveFile", fileId);
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }
        }

        public async Task LoadMessages(int chatId, int messagesCount)
        {
            try
            {
                var messages = await _context.Messages
                    .OrderByDescending(x => x.Id)
                    .Where(x => x.ChatId == chatId)
                    .Skip(messagesCount)
                    .Take(30)
                    .ToListAsync();
                
                await Clients.Caller.SendAsync("ReceiveMessage", messages, "0", chatId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //public async Task SendMessageGroup(string groupName, string message)
        //{
        //    await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        //}

        //public async Task AddToGroup(string groupName)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //}

        //public async Task RemoveFromGroup(string groupName)
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        //}
    }
}