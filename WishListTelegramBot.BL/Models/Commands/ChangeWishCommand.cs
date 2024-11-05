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
    public class ChangeWishCommand : BuisnessLogicCommand, ICommand, IListener
    {

        public CommandExecutor Executor { get; set; }
        private OperationType _operationType;
        private int _id;

        public ChangeWishCommand(Bot bot, CommandExecutor executor, DataBaseConnector dataBaseConnector) : base(bot, dataBaseConnector)
        {
            Executor = executor;
        }

        public override List<string> Name => new List<string> { "/changewish" };


        public override async Task Execute(Update update)
        {
            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;

            if (update.Message == null && update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && !string.IsNullOrEmpty(update.CallbackQuery.Data))
            {
                CallBackData? callBackData = JsonSerializer.Deserialize<CallBackData>(update.CallbackQuery.Data);
                if (callBackData != null && !string.IsNullOrEmpty(callBackData.Data))
                {
                    WishCallBackData? wishCallBackData = JsonSerializer.Deserialize<WishCallBackData>(callBackData.Data);
                    _id = wishCallBackData.Id;
                    _operationType = wishCallBackData.Operation;
                }
            }
            await Client.DeleteMessageAsync(chatId, update.CallbackQuery.Message.MessageId);
            switch (_operationType)
            {
                case OperationType.ChangeName:
                    {
                        await Client.SendTextMessageAsync(chatId, "Введите название вашего желания (Для отмены нажмите /exit)");
                    }
                break;
                case OperationType.ChangeDescription:
                    {
                        await Client.SendTextMessageAsync(chatId, "Введите описание вашего желания (Для отмены нажмите /exit)");
                    }
                    break;
                case OperationType.ChangeLink:
                    {
                        await Client.SendTextMessageAsync(chatId, "Введите ссылку на ваше желание (Для отмены нажмите /exit)");
                    }
                    break;
                case OperationType.ChangePhoto:
                    {
                        await Client.SendTextMessageAsync(chatId, "Прилите фотографию вашего желания (Для отмены нажмите /exit)");
                    }
                    break;
            }
            Executor.StartListen(this); //говорим, что теперь нам надо отправлять апдейты
        }

        public async Task GetUpdate(Update update)
        {
            try
            {
                Executor.StopListen();
                string text = update.Message.Text;
                long chatId = update.Message.Chat.Id;

                var wish = await DataBaseConnector.WishService.GetWishByIdAsync(_id);
                List<List<CommandCallBackItem>> commandCallBackItems = new List<List<CommandCallBackItem>>();
                List<CommandCallBackItem> callBackItemsLine = GetBackButton(wish.Id);
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
                            wish.Name = text;
                        }
                        break;
                    case OperationType.ChangeDescription:
                        {
                            wish.Description = text;
                        }
                        break;
                    case OperationType.ChangeLink:
                        {
                            wish.Link = text;
                        }
                        break;
                    case OperationType.ChangePhoto:
                        {

                            if (update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
                            {
                                string fileId = update.Message.Photo.Last().FileId;
                                wish.PhotoId = fileId;
                            }
                            else
                            {
                                await Client.SendTextMessageAsync(chatId, "Фотография не подходит", replyMarkup: keyboard);
                            }
                            break;
                        }
                }

                await DataBaseConnector.WishService.UpdateWishAsync(wish);

                await Client.SendTextMessageAsync(chatId, "Желание " + wish.Name + " изменено", replyMarkup: keyboard);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<CommandCallBackItem> GetBackButton(int wishId)
        {
            List<CommandCallBackItem> callBackItemsLine = new List<CommandCallBackItem>();
            callBackItemsLine.Add(new CommandCallBackItem()
            {
                Name = "Назад",
                CallBackData = new CallBackData()
                {
                    Command = "/infowish",
                    Data = wishId.ToString()
                }
            });
            return callBackItemsLine;
        }
    }
}