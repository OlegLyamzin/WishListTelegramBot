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

    public class DeleteWishCommand : BuisnessLogicCommand, ICommand
    {
        public DeleteWishCommand(Bot bot, DataBaseConnector serverConnector) : base(bot, serverConnector)
        {
        }

        public override List<string> Name => new List<string> { "/deletewish" };


        public override async Task Execute(Update update)
        {
            int id = 0;
            if (update.Message == null && update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && !string.IsNullOrEmpty(update.CallbackQuery.Data))
            {
                CallBackData? callBackData = JsonSerializer.Deserialize<CallBackData>(update.CallbackQuery.Data);
                id = Convert.ToInt32(callBackData.Data);

            }

            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;

            var wish = await DataBaseConnector.WishService.GetWishByIdAsync(id);
            var wishLists = await DataBaseConnector.WishListService.GetWishListByIdAsync((int)wish.WishListId);
            var user = await DataBaseConnector.UserService.GetUserByIdAsync((int)wishLists.UserId);
            var giver = await DataBaseConnector.UserService.GetUserByTgIdAsync(chatId.ToString());
            wish.GiverId = giver.Id;
            List<List<CommandCallBackItem>> callBackItems = GetCommandModule((int)wish.WishListId);
            DataBaseConnector.WishService.DeleteWish(wish.Id);
            var keyboard = Keyboards.GetCallBackMenuItems(callBackItems);
            try
            {
                await Client.DeleteMessageAsync(chatId, update.CallbackQuery.Message.MessageId);
            }
            catch (Exception ex) { }
            await Client.SendTextMessageAsync(chatId, "Желание " + wish.Name + " удалено", replyMarkup: (InlineKeyboardMarkup?)keyboard);

        }

        private List<List<CommandCallBackItem>> GetCommandModule(int wishListId)
        {

            var callBackItems = new List<List<CommandCallBackItem>>();
            var callBackItemsLine = new List<CommandCallBackItem>();

            callBackItems.Add(callBackItemsLine);
            var json = JsonSerializer.Serialize(new WishListCallBackData
            {
                Id = wishListId
            });
            callBackItemsLine.Add(new CommandCallBackItem()
            {
                Name = "Назад",
                CallBackData = new CallBackData()
                {
                    Command = "/infowishlist",
                    Data = json
                }
            });
            return callBackItems;
        }


    }
}

