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

    public class GetWishListCommand : BuisnessLogicCommand, ICommand
    {
        public GetWishListCommand(Bot bot, DataBaseConnector serverConnector) : base(bot, serverConnector)
        {
        }

        public override List<string> Name => new List<string> { "/wishlist" };
        private int _itemsInPage = 6;


        public override async Task Execute(Update update)
        {
            int page = 0;
            int id = 0;
            if (update.Message == null && update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && !string.IsNullOrEmpty(update.CallbackQuery.Data))
            {
                CallBackData? callBackData = JsonSerializer.Deserialize<CallBackData>(update.CallbackQuery.Data);
                if (callBackData != null && !string.IsNullOrEmpty(callBackData.Data))
                {
                    WishListCallBackData? wishListCallBackData = JsonSerializer.Deserialize<WishListCallBackData>(callBackData.Data);
                    page = Convert.ToInt32(wishListCallBackData.Page);
                    id = Convert.ToInt32(wishListCallBackData.Id);
                }
            }

            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;
            var user = await DataBaseConnector.UserService.GetUserByTgIdAsync(chatId.ToString());
            var wishLists = await DataBaseConnector.WishListService.GetWishListByIdAsync(id);
            var wishes = await DataBaseConnector.WishService.GetWishesByWishListIdAsync(id);

            int pages = (wishes.Count / _itemsInPage) + (wishes.Count % _itemsInPage == 0 ? 0 : 1);

            List<List<CommandCallBackItem>> callBackItems = GetWishesListModule(page, wishes);
            var keyboard = Keyboards.GetCallBackMenuItems(callBackItems);
            List<CommandCallBackItem> callBackItemsLine = GetPaginationModule(page, id, pages);
            keyboard = Keyboards.AddCallBackMenuItems((InlineKeyboardMarkup)keyboard, callBackItemsLine);
            callBackItemsLine = GetBackButton(id);
            keyboard = Keyboards.AddCallBackMenuItems((InlineKeyboardMarkup)keyboard, callBackItemsLine);
            string wishesString = GetWishListAsString(wishes);
            try
            {
                await Client.DeleteMessageAsync(chatId, update.CallbackQuery.Message.MessageId);
            }
            catch (Exception ex) { }
            await Client.SendTextMessageAsync(chatId, $"Список желаний в вишлисте \r\n*{wishLists.Name}*\r\n\r\n{wishesString}", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (InlineKeyboardMarkup?)keyboard);
        }

        private List<CommandCallBackItem> GetBackButton(int id)
        {
            List<CommandCallBackItem> callBackItemsLine = new List<CommandCallBackItem>();
            var json = JsonSerializer.Serialize(new WishListCallBackData
            {
                Id = (int)id,
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

        private string GetWishListAsString(List<Wish> wishes)
        {
            var result = new StringBuilder();
            foreach (var wish in wishes) 
            {
                result.Append(wish.Name + (wish.GiverId is null || wish.GiverId == 0 ? "" : " *(забронировано)*") + "\r\n");
            }
            return result.ToString();
        }

        private List<List<CommandCallBackItem>> GetWishesListModule(int page, List<Wish> wishes)
        {
            var callBackItems = new List<List<CommandCallBackItem>>();
            var callBackItemsLine = new List<CommandCallBackItem>();
            callBackItems.Add(callBackItemsLine);
            for (int i = page * _itemsInPage; i < ((page + 1) * _itemsInPage) && i < wishes.Count; i++)
            {
                if (i % 2 == 0 && i != 0)
                {
                    callBackItemsLine = new List<CommandCallBackItem>();
                    callBackItems.Add(callBackItemsLine);
                }
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = wishes[i].Name,
                    CallBackData = new CallBackData()
                    {
                        Command = "/infowish",
                        Data = wishes[i].Id.ToString()
                    }
                });
            }
            return callBackItems;
        }

        private List<CommandCallBackItem> GetPaginationModule(int page, int id, int pages)
        {
            List<CommandCallBackItem> callBackItemsLine = new List<CommandCallBackItem>();
            if (page != 0)
            {
                var json = JsonSerializer.Serialize(new WishListCallBackData
                {
                    Id = id,
                    Page = (page - 1)
                });
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = "<<",
                    CallBackData = new CallBackData()
                    {
                        Command = "/wishlist",
                        Data = json
                    }
                });
            }
            //for (int i = page - 1; i < page + 5 && i < pages; i++)
            //{
            //    if (i != page && i >= 0)
            //    {
            //        var json = JsonSerializer.Serialize(new WishListCallBackData
            //        {
            //            Id = id,
            //            Page = i
            //        });
            //        callBackItemsLine.Add(new CommandCallBackItem()
            //        {
            //            Name = (i + 1).ToString(),
            //            CallBackData = new CallBackData()
            //            {
            //                Command = "/wishlist",
            //                Data = json
            //            }
            //        });
            //    }
            //}
            if (page + 1 < pages)
            {
                var json = JsonSerializer.Serialize(new WishListCallBackData
                {
                    Id = id,
                    Page = (page + 1)
                });
                callBackItemsLine.Add(new CommandCallBackItem()
                {
                    Name = ">>",
                    CallBackData = new CallBackData()
                    {
                        Command = "/wishlist",
                        Data = json
                    }
                });
            }

            return callBackItemsLine;
        }
    }
}
