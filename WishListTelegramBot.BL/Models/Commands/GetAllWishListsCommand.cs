using DocumentFormat.OpenXml.Office2010.ExcelAc;
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
    
    public class GetAllWishListsCommand : BuisnessLogicCommand, ICommand
    {
        public GetAllWishListsCommand(Bot bot, DataBaseConnector serverConnector) : base(bot, serverConnector)
        {
        }

        public override List<string> Name => new List<string> { "/mywishlists", "Мои вишлисты 🤔" };


        public override async Task Execute(Update update)
        {
            int page = 0;
            if (update.Message == null && update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && !string.IsNullOrEmpty(update.CallbackQuery.Data))
            {
                CallBackData? callBackData = JsonSerializer.Deserialize<CallBackData>(update.CallbackQuery.Data);
                page = Convert.ToInt32(callBackData.Data);
            }

            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;
            var user = await DataBaseConnector.UserService.GetUserByTgIdAsync(chatId.ToString());
            var wishLists = await DataBaseConnector.WishListService.GetWishListByUserIdAsync(user.Id);
            int pages = (wishLists.Count / 9) + (wishLists.Count % 9 == 0 ? 0 : 1);

            List<List<CommandCallBackItem>> callBackItems = GetWishListsModule(page, wishLists);
            var keyboard = Keyboards.GetCallBackMenuItems(callBackItems);
            List<CommandCallBackItem> callBackItemsLine = GetPaginationModule(page, pages);
            keyboard = Keyboards.AddCallBackMenuItems((InlineKeyboardMarkup)keyboard, callBackItemsLine);

            await Client.SendTextMessageAsync(chatId, @"Ваши виш листы", replyMarkup: keyboard);
        }

        private  List<CommandCallBackItem> GetPaginationModule(int page, int pages)
        {
            List<CommandCallBackItem> callBackItemsLine = new List<CommandCallBackItem>();
            if (page != 0)
            {
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = "<<",
                    CallBackData = new CallBackData()
                    {
                        Command = "/mywishlists",
                        Data = (page - 1).ToString()
                    }
                });
            }
            for (int i = page - 1; i < page + 5 && i < pages; i++)
            {
                if (i != page && i >= 0)
                {
                    callBackItemsLine.Add(new CommandCallBackItem()
                    {
                        Name = (i + 1).ToString(),
                        CallBackData = new CallBackData()
                        {
                            Command = "/mywishlists",
                            Data = i.ToString()
                        }
                    });
                }
            }
            if (page + 1 < pages)
            {
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = ">>",
                    CallBackData = new CallBackData()
                    {
                        Command = "/mywishlists",
                        Data = (page + 1).ToString()
                    }
                });
            }

            return callBackItemsLine;
        }

        private List<List<CommandCallBackItem>> GetWishListsModule(int page, List<WishList> wishLists)
        {
            var callBackItems = new List<List<CommandCallBackItem>>();
            var callBackItemsLine = new List<CommandCallBackItem>();
            callBackItems.Add(callBackItemsLine);
            for (int i = page * 9; i < ((page + 1) * 9) && i < wishLists.Count; i++)
            {
                if (i % 3 == 0 && i != 0)
                {
                    callBackItemsLine = new List<CommandCallBackItem>();
                    callBackItems.Add(callBackItemsLine);
                }
                var json = JsonSerializer.Serialize(new WishListCallBackData
                {
                    Id = wishLists[i].Id
                });
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = wishLists[i].Name,
                    CallBackData = new CallBackData()
                    {
                        Command = "/infowishlist",
                        Data = json
                    }
                });
            }
            return callBackItems;
        }
    }
}
