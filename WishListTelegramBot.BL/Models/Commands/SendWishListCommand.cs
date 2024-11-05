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

    public class SendWishListCommand : BuisnessLogicCommand, ICommand
    {
        public SendWishListCommand(Bot bot, DataBaseConnector serverConnector) : base(bot, serverConnector)
        {
        }

        public override List<string> Name => new List<string> { "/sendwishlist" };


        public override async Task Execute(Update update)
        {
            int id = 0;
            if (update.Message == null && update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && !string.IsNullOrEmpty(update.CallbackQuery.Data))
            {
                CallBackData? callBackData = JsonSerializer.Deserialize<CallBackData>(update.CallbackQuery.Data);
                id = Convert.ToInt32(callBackData.Data);

            }

            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;
            List<List<CommandCallBackItem>> callBackItems = GetBackButton(id);
            var keyboard = Keyboards.GetCallBackMenuItems(callBackItems);
            try
            {
                await Client.DeleteMessageAsync(chatId, update.CallbackQuery.Message.MessageId);
            }
            catch (Exception ex) { }
            await Client.SendTextMessageAsync(chatId, @"Я создал для тебя пригласительную открытку. Просто перешли ее своим друзьям!" + id, replyMarkup: (InlineKeyboardMarkup?)keyboard);
            await Client.SendPhotoAsync(chatId, new InputFileId("AgACAgIAAxkBAAIDQmcAAfSA42JIAizE-VYp4Ewl1MPJWAACt-ExG38NAUjHVXxpz2YtoAEAAwIAA3gAAzYE"),caption: @$"Привет!

Я веду свой вишлист с помощью бота [Just WishList Bot](https://t.me/JustWishListBot?start={id})✨

Мой вишлист: https://t.me/JustWishListBot?start={id}

Бот позволяет смотреть вишлисты пользователей, бронировать желания и добавлять свои!

Присоединяйся!
@JustWishListBot
", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }

        private List<List<CommandCallBackItem>> GetBackButton(int wishlistid)
        {

            var callBackItems = new List<List<CommandCallBackItem>>();
            var callBackItemsLine = new List<CommandCallBackItem>();
            
            callBackItems.Add(callBackItemsLine);
            var json = JsonSerializer.Serialize(new WishListCallBackData
            {
                Id = (int)wishlistid,
                Page = 0
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

