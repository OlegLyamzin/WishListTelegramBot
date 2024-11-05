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

namespace WishListTelegramBot.BL.Models.Commands
{
    public class CreateWishListCommand : BuisnessLogicCommand, ICommand, IListener
    {

        public CommandExecutor Executor { get; set; }
        private string _name;

        public CreateWishListCommand(Bot bot, CommandExecutor executor, DataBaseConnector dataBaseConnector) : base(bot, dataBaseConnector)
        {
            Executor = executor;
        }

        public override List<string> Name => new List<string> { "/createwishlist", "Добавить виш лист ➕" };


        public override async Task Execute(Update update)
        {
            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;
            

            await Client.SendTextMessageAsync(chatId, "Введите название вашего нового вишлиста (Для отмены нажмите /exit)");
            Executor.StartListen(this); //говорим, что теперь нам надо отправлять апдейты
        }

        public async Task GetUpdate(Update update)
        {
            try
            {
                Executor.StopListen();
                string text = update.Message.Text;
                long chatId = update.Message.Chat.Id;

                if (text == "/exit")
                {
                    _name = null;
                    return;
                }
                _name = text;
                var user = await DataBaseConnector.UserService.GetUserByTgIdAsync(chatId.ToString());
                var wishlist = await DataBaseConnector.WishListService.CreateWishListAsync(new WishList
                {
                    Name = _name,
                    UserId =  user.Id
                });
                var json = JsonSerializer.Serialize(new WishListCallBackData
                {
                    Id = wishlist.Id,
                    Page = 0
                });
                var keyboard = Keyboards.GetCallBackMenuItems(new List<List<CommandCallBackItem>> { new List<CommandCallBackItem> { new CommandCallBackItem { Name = "Открыть вишлист", CallBackData = new CallBackData { Command = "/infowishlist", Data = json } } } });
                await Client.SendTextMessageAsync(chatId, "Ваш вишлист " + _name + " создан", replyMarkup: keyboard);

            }
            catch (Exception ex)
            {
                _name = null;
                throw ex;
            }
        }

    }
}
