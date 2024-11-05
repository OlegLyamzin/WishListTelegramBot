using WishListTelegramBot.BL;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.Json;
using DocumentFormat.OpenXml.Spreadsheet;
using WishListTelegramBot.DL.Models;

namespace WishListTelegramBot.BL.Models.Commands
{
    public class MainMenuCommand : BuisnessLogicCommand, ICommand
    {
        public MainMenuCommand(Bot bot, DataBaseConnector serverConnector) : base(bot, serverConnector)
        {
        }

        public override List<string> Name => new List<string> { "Главное меню  ↩️", "/start" };


        public override async Task Execute(Update update)
        {
            long chatId = update.Message.Chat.Id;
            string username = update.Message.Chat.Username == null ? update.Message.Chat.Id + "_" + update.Message.Chat.FirstName+"_"+update.Message.Chat.LastName : update.Message.Chat.Username;

                await Client.SendTextMessageAsync(Bot.GetMainAdmin(), $" {chatId} - @{update.Message.Chat.Username} - {update.Message.Chat.FirstName} - {update.Message.Chat.LastName} - Попытка нового подключения");
            var user = await DataBaseConnector.UserService.GetUserByTgIdAsync(chatId.ToString());
            if (user == null)
            {
                await DataBaseConnector.UserService.CreateUserAsync(new DL.Models.User
                {
                    TgId = chatId.ToString(),
                    Username = update.Message.Chat.Username,
                    Name = update.Message.Chat.FirstName,
                    Lastname = update.Message.Chat.LastName
                });
            }

            var message = await Client.SendTextMessageAsync(chatId, "Добро пожаловать в бот для создания вишлистов", replyMarkup: Keyboards.GetMainMenuBoard());
            var argsStart = update.Message.Text.Split(" ");
            if (argsStart.Length > 1)
            {
                string wishlistid = argsStart[1];
                var wishList = await DataBaseConnector.WishListService.GetWishListByIdAsync(Convert.ToInt32(wishlistid));
                var keyboard = Keyboards.GetCallBackMenuItems(GetOpenButton(wishList.Id));
                try
                {
                    await Client.DeleteMessageAsync(chatId, message.MessageId);
                }catch (Exception ex) { }
                await Client.SendTextMessageAsync(chatId, "Вам отправили вишлист, так скорее его посмотрите! \r\n Нажмите на кнопку", replyMarkup: (InlineKeyboardMarkup?)keyboard);
            }
        }

        private List<List<CommandCallBackItem>> GetOpenButton(int id)
        {
            List<List<CommandCallBackItem>> callBackItems = new List<List<CommandCallBackItem>>();
            List<CommandCallBackItem> callBackItemsLine = new List<CommandCallBackItem>();
            callBackItems.Add(callBackItemsLine);
            var json = JsonSerializer.Serialize(new WishListCallBackData
            {
                Id = (int)id,
                Page = 0
            });
            callBackItemsLine.Add(new CommandCallBackItem()
            {
                Name = "Открыть вишлист",
                CallBackData = new CallBackData()
                {
                    Command = "/wishlist",
                    Data = json
                }
            });
            return callBackItems;
        }

    }
}
