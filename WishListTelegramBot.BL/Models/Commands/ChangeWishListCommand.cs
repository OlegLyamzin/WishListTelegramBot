using Telegram.Bot.Types;
using Telegram.Bot;
using WishListTelegramBot.DL.Models;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Net;
using System;
using System.IO;
using IronQr;
using QRCoder;
using System.Text.Json;
using DocumentFormat.OpenXml.Office2010.Excel;
using Telegram.Bot.Types.ReplyMarkups;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WishListTelegramBot.BL.Models.Commands
{
    public class ChangeWishListCommand : BuisnessLogicCommand, ICommand, IListener
    {

        public CommandExecutor Executor { get; set; }
        private OperationType _operationType;
        private int _id;

        public ChangeWishListCommand(Bot bot, CommandExecutor executor, DataBaseConnector dataBaseConnector) : base(bot, dataBaseConnector)
        {
            Executor = executor;
        }

        public override List<string> Name => new List<string> { "/changewishlist" };


        public override async Task Execute(Update update)
        {
            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;

            if (update.Message == null && update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && !string.IsNullOrEmpty(update.CallbackQuery.Data))
            {
                CallBackData? callBackData = JsonSerializer.Deserialize<CallBackData>(update.CallbackQuery.Data);
                _id = Convert.ToInt32(callBackData.Data);
                _operationType = OperationType.ChangeName;
            }
            await Client.DeleteMessageAsync(chatId, update.CallbackQuery.Message.MessageId);
            switch (_operationType)
            {
                case OperationType.ChangeName:
                    {
                        await Client.SendTextMessageAsync(chatId, "Введите название вашего вишлиста (Для отмены нажмите /exit)");
                        Executor.StartListen(this); //говорим, что теперь нам надо отправлять апдейты
                    }
                    break;
            }
        }

        public async Task GetUpdate(Update update)
        {
            try
            {
                Executor.StopListen();
                string text = update.Message.Text;
                long chatId = update.Message.Chat.Id;

                var wishList = await DataBaseConnector.WishListService.GetWishListByIdAsync(_id);
                List<List<CommandCallBackItem>> commandCallBackItems = new List<List<CommandCallBackItem>>();
                List<CommandCallBackItem> callBackItemsLine = GetBackButton(wishList.Id);
                commandCallBackItems.Add(callBackItemsLine);
                var keyboard = Keyboards.GetCallBackMenuItems(commandCallBackItems);
                if (text == "/exit")
                {
                    await Client.SendTextMessageAsync(chatId, "Отменено", replyMarkup: keyboard);
                    return;
                }

                switch (_operationType)
                {
                    case OperationType.ChangeName:
                        {
                            wishList.Name = text;
                        }
                        break;
                }

                await DataBaseConnector.WishListService.UpdateWishListAsync(wishList);

                await Client.SendTextMessageAsync(chatId, "Вишлист " + wishList.Name + " изменен", replyMarkup: keyboard);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<CommandCallBackItem> GetBackButton(int wishListId)
        {
            List<CommandCallBackItem> callBackItemsLine = new List<CommandCallBackItem>();
            var json = JsonSerializer.Serialize(new WishListCallBackData
            {
                Id = (int)wishListId,
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
            return callBackItemsLine;
        }
    }
}