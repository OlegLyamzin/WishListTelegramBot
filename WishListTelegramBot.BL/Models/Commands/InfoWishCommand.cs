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

    public class InfoWishCommand : BuisnessLogicCommand, ICommand
    {
        public InfoWishCommand(Bot bot, DataBaseConnector serverConnector) : base(bot, serverConnector)
        {
        }

        public override List<string> Name => new List<string> { "/infowish" };


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
            bool isGiver = false;
            if (wish.GiverId != null && wish.GiverId != 0)
            {
                var giver = await DataBaseConnector.UserService.GetUserByIdAsync((int)wish.GiverId);
                isGiver = giver.TgId == chatId.ToString();
            }

            List<List<CommandCallBackItem>> callBackItems = GetCommandModule(wish, user.TgId == chatId.ToString(), isGiver);
            var keyboard = Keyboards.GetCallBackMenuItems(callBackItems);


            string messageText = @$"*{wish.Name}*" + (!string.IsNullOrEmpty(wish.Description) ? $"\r\n\r\n*Описание*: {wish.Description}" : "") + (!string.IsNullOrEmpty(wish.Link) ? $"\r\n\r\n*Ссылка*: {wish.Link}" : "");
            
            try
            {
                await Client.DeleteMessageAsync(chatId, update.CallbackQuery.Message.MessageId);
            }
            catch (Exception ex) { }
            if (string.IsNullOrEmpty(wish.PhotoId))
            {
                await Client.SendTextMessageAsync(chatId, messageText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (InlineKeyboardMarkup?)keyboard);
            }
            else
            {
                await Client.SendPhotoAsync(chatId, new InputFileId(wish.PhotoId), caption: messageText, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (InlineKeyboardMarkup?)keyboard);
            }
        }

        private List<List<CommandCallBackItem>> GetCommandModule(Wish wish, bool isWisher, bool isGiver)
        {

            var callBackItems = new List<List<CommandCallBackItem>>();
            var callBackItemsLine = new List<CommandCallBackItem>();
            if (wish.GiverId == null || wish.GiverId == 0 || isGiver)
            {
                callBackItems.Add(callBackItemsLine);
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = isGiver? "Отменить бронь":"Забронировать",
                    CallBackData = new CallBackData()
                    {
                        Command = "/bookwish",
                        Data = wish.Id.ToString()
                    }
                });
            }
            if (isWisher)
            {
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = "Удалить",
                    CallBackData = new CallBackData()
                    {
                        Command = "/deletewish",
                        Data = wish.Id.ToString()
                    }
                });
                callBackItemsLine = new List<CommandCallBackItem>();
                callBackItems.Add(callBackItemsLine);
                var wishCallBack = new WishCallBackData
                {
                    Id = (int)wish.Id,
                    Operation = OperationType.ChangeName
                };
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = "Изменить название",
                    CallBackData = new CallBackData()
                    {
                        Command = "/changewish",
                        Data = JsonSerializer.Serialize(wishCallBack)
                    }
                });


                wishCallBack.Operation = OperationType.ChangeDescription;
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = "Изменить описание",
                    CallBackData = new CallBackData()
                    {
                        Command = "/changewish",
                        Data = JsonSerializer.Serialize(wishCallBack)
                    }
                });

                callBackItemsLine = new List<CommandCallBackItem>();
                callBackItems.Add(callBackItemsLine);
                wishCallBack.Operation = OperationType.ChangePhoto;
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = "Изменить картинку",
                    CallBackData = new CallBackData()
                    {
                        Command = "/changewish",
                        Data = JsonSerializer.Serialize(wishCallBack)
                    }
                });

                wishCallBack.Operation = OperationType.ChangeLink;
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = "Изменить ссылку",
                    CallBackData = new CallBackData()
                    {
                        Command = "/changewish",
                        Data = JsonSerializer.Serialize(wishCallBack)
                    }
                });
            }

            callBackItemsLine = new List<CommandCallBackItem>();
            callBackItems.Add(callBackItemsLine);
            var json = JsonSerializer.Serialize(new WishListCallBackData
            {
                Id = (int)wish.WishListId,
                Page = 0
            });
            callBackItemsLine.Add(new CommandCallBackItem()
            {
                Name = "Назад",
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

