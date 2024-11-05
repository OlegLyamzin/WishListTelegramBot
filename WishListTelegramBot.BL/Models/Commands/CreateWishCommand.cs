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

namespace WishListTelegramBot.BL.Models.Commands
{
    public class CreateWishCommand : BuisnessLogicCommand, ICommand, IListener
    {

        public CommandExecutor Executor { get; set; }
        private string _name;
        private int _wishListId = 0;

        public CreateWishCommand(Bot bot, CommandExecutor executor, DataBaseConnector dataBaseConnector) : base(bot, dataBaseConnector)
        {
            Executor = executor;
        }

        public override List<string> Name => new List<string> { "/createwish" };


        public override async Task Execute(Update update)
        {
            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;

            if (update.Message == null && update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && !string.IsNullOrEmpty(update.CallbackQuery.Data))
            {
                CallBackData? callBackData = JsonSerializer.Deserialize<CallBackData>(update.CallbackQuery.Data);
                _wishListId = Convert.ToInt32(callBackData.Data);
            }
            await Client.SendTextMessageAsync(chatId, "Введите название вашего желания (Для отмены нажмите /exit)");
            Executor.StartListen(this); //говорим, что теперь нам надо отправлять апдейты
        }

        public async Task GetUpdate(Update update)
        {
            try
            {
                Executor.StopListen();
                string text = update.Message.Text;
                long chatId = update.Message.Chat.Id;

                List<List<CommandCallBackItem>> commandCallBackItems = new List<List<CommandCallBackItem>>();
                List<CommandCallBackItem> callBackItemsLine = GetBackButton(_wishListId);
                commandCallBackItems.Add(callBackItemsLine);
                var keyboard = Keyboards.GetCallBackMenuItems(commandCallBackItems);
                if (text == "/exit")
                {
                    _name = null;
                    await Client.SendTextMessageAsync(chatId, "Отменено", replyMarkup: keyboard);
                    return;
                }
                _name = text;
                var user = await DataBaseConnector.UserService.GetUserByTgIdAsync(chatId.ToString());
                var wish = await DataBaseConnector.WishService.CreateWishAsync(new Wish
                {
                    Name = _name,
                    WishListId = _wishListId
                });
                keyboard = Keyboards.AddCallBackMenuItems((InlineKeyboardMarkup)keyboard, new List<CommandCallBackItem> { new CommandCallBackItem { Name = "Добавить подробности", CallBackData = new CallBackData { Command = "/infowish", Data = wish.Id.ToString() } } });
                await Client.SendTextMessageAsync(chatId, "Желание " + _name + " создано", replyMarkup: keyboard);

            }
            catch (Exception ex)
            {
                _name = null;
                throw ex;
            }
        }

        private List<CommandCallBackItem> GetBackButton(int wishlistId)
        {
            List<CommandCallBackItem> callBackItemsLine = new List<CommandCallBackItem>();
            var json = JsonSerializer.Serialize(new WishListCallBackData
            {
                Id = (int)wishlistId,
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