using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WishListTelegramBot.DL.Models;

namespace WishListTelegramBot.BL.Models.Commands
{

    public class InfoWishListCommand : BuisnessLogicCommand, ICommand
    {
        public InfoWishListCommand(Bot bot, DataBaseConnector serverConnector) : base(bot, serverConnector)
        {
        }

        public override List<string> Name => new List<string> { "/infowishlist" };


        public override async Task Execute(Update update)
        {
            int id = 0;
            if (update.Message == null && update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && !string.IsNullOrEmpty(update.CallbackQuery.Data))
            {
                CallBackData? callBackData = JsonSerializer.Deserialize<CallBackData>(update.CallbackQuery.Data);
                if (callBackData != null && !string.IsNullOrEmpty(callBackData.Data))
                {
                    WishListCallBackData? wishListCallBackData = JsonSerializer.Deserialize<WishListCallBackData>(callBackData.Data);
                    id = Convert.ToInt32(wishListCallBackData.Id);
                }
            }

            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;
            var wishLists = await DataBaseConnector.WishListService.GetWishListByIdAsync(id);
            var user = await DataBaseConnector.UserService.GetUserByIdAsync((int)wishLists.UserId);
            var wishes = await DataBaseConnector.WishService.GetWishesByWishListIdAsync(id);

            List<List<CommandCallBackItem>> callBackItems = GetCommandModule(wishLists, user.TgId == chatId.ToString());
            var keyboard = Keyboards.GetCallBackMenuItems(callBackItems);

            string wishesString = GetWishListAsString(wishes);
            try
            {
                await Client.DeleteMessageAsync(chatId, update.CallbackQuery.Message.MessageId);
            }
            catch (Exception ex) { }
            await Client.SendTextMessageAsync(chatId, $"Список желаний в вишлисте \r\n*{wishLists.Name}*\r\n\r\n{wishesString}", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (InlineKeyboardMarkup?)keyboard);
        }

        private List<List<CommandCallBackItem>> GetCommandModule(WishList wishList, bool isWisher)
        {

            var callBackItems = new List<List<CommandCallBackItem>>();
            var callBackItemsLine = new List<CommandCallBackItem>();
            callBackItems.Add(callBackItemsLine);
                var json = JsonSerializer.Serialize(new WishListCallBackData
                {
                    Id = wishList.Id,
                    Page = 0
                });
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = "Подробнее о желании",
                    CallBackData = new CallBackData()
                    {
                        Command = "/wishlist",
                        Data = json
                    }
                });
            if (isWisher)
            {
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = "Добавить желание",
                    CallBackData = new CallBackData()
                    {
                        Command = "/createwish",
                        Data = wishList.Id.ToString()
                    }
                });

                callBackItemsLine = new List<CommandCallBackItem>();
                callBackItems.Add(callBackItemsLine);

                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = "Изменить название",
                    CallBackData = new CallBackData()
                    {
                        Command = "/changewishlist",
                        Data = wishList.Id.ToString()
                    }
                });
                //callBackItemsLine.Add(new CommandCallBackItem()
                //{
                //    Name = "Удалить вишлист",
                //    CallBackData = new CallBackData()
                //    {
                //        Command = "/deletewishlist",
                //        Data = wishList.Id.ToString()
                //    }
                //});
                callBackItemsLine = new List<CommandCallBackItem>();
                callBackItems.Add(callBackItemsLine);

                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = "Поделиться вишлистом",
                    CallBackData = new CallBackData()
                    {
                        Command = "/sendwishlist",
                        Data = wishList.Id.ToString()
                    }
                });
            }

            return callBackItems;
        }

        private string GetWishListAsString(List<Wish> wishes)
        {
            var result = new StringBuilder();
            foreach (var wish in wishes)
            {
                result.Append(wish.Name + (wish.GiverId is null || wish.GiverId == 0 ? "" : " *(забронировано)*") + "\r\n");
            }
            return result.ToString();
        }

    }
}
