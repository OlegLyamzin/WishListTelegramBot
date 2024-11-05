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

    public class DeleteWishListCommand : BuisnessLogicCommand, ICommand
    {
        public DeleteWishListCommand(Bot bot, DataBaseConnector serverConnector) : base(bot, serverConnector)
        {
        }

        public override List<string> Name => new List<string> { "/deletewishlist" };


        public override async Task Execute(Update update)
        {
            int id = 0;
            if (update.Message == null && update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && !string.IsNullOrEmpty(update.CallbackQuery.Data))
            {
                CallBackData? callBackData = JsonSerializer.Deserialize<CallBackData>(update.CallbackQuery.Data);
                id = Convert.ToInt32(callBackData.Data);

            }

            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;

            var wishLists = await DataBaseConnector.WishListService.GetWishListByIdAsync(id);
            string name = wishLists.Name;
            foreach(var wish in wishLists.Wishes)
            {
                DataBaseConnector.WishService.DeleteWish(wish.Id);
            }
            DataBaseConnector.WishListService.DeleteWishList(id);
            List<List<CommandCallBackItem>> callBackItems = GetCommandModule();
            var keyboard = Keyboards.GetCallBackMenuItems(callBackItems);
            try
            {
                await Client.DeleteMessageAsync(chatId, update.CallbackQuery.Message.MessageId);
            }
            catch (Exception ex) { }
            await Client.SendTextMessageAsync(chatId, "Вишлист " + name + " удален", replyMarkup: (InlineKeyboardMarkup?)keyboard);

        }

        private List<List<CommandCallBackItem>> GetCommandModule()
        {

            var callBackItems = new List<List<CommandCallBackItem>>();
            var callBackItemsLine = new List<CommandCallBackItem>();

            callBackItems.Add(callBackItemsLine);
            callBackItemsLine.Add(new CommandCallBackItem()
            {
                Name = "К списку вишлистов",
                CallBackData = new CallBackData()
                {
                    Command = "/mywishlists",
                    Data = "0"
                }
            });
            return callBackItems;
        }


    }
}

