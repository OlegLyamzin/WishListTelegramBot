using WishListTelegramBot.BL.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WishListTelegramBot.DL.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Text.Json;
using Newtonsoft.Json;

namespace WishListTelegramBot.BL.Models
{
    public static class Keyboards
    {
        public static ReplyKeyboardMarkup GetMainMenuBoard()
        {
            var kbrd = new ReplyKeyboardMarkup(
                new[] {
                    new[] 
                    {
                        new KeyboardButton("Пожертвовать ❤️"),
                        new KeyboardButton("Добавить виш лист ➕"),
                    },
                    new[] 
                    {
                        new KeyboardButton("Мои вишлисты 🤔")
                    }
                    
                }
                )
            {
                ResizeKeyboard = true
            };
            return kbrd;
        }

        public static ReplyKeyboardMarkup GetMainMenuBoard(bool isAdmin)
        {
            string adminCommand = isAdmin ? "Админка 🤡" : "Написать админу 🤡";
            var kbrd = new ReplyKeyboardMarkup(
                new[] {
                    new[]
                    {
                        new KeyboardButton("Добавить доход (+)")
                    },
                    new[] 
                    {
                        new KeyboardButton("Добавить расход (-)")
                    },
                    new[]
                    {
                        new KeyboardButton("Добавить накопления (=)")
                    },
                    new[] 
                    {
                        new KeyboardButton("Получить Эксель")
                    }
                    //new[]
                    //{
                    //    new KeyboardButton(adminCommand)
                    //}

                }
                )
            {
                ResizeKeyboard = true
            };
            return kbrd;
        }


        internal static IReplyMarkup? GetDonateInfoBoard()
        {
            var kbrd = new InlineKeyboardMarkup(new InlineKeyboardButton[][]
            {
                new []  {
                        InlineKeyboardButton.WithUrl("Tribute", "https://t.me/tribute/app?startapp=dbWE")
                        }

                //new []  {
                //        InlineKeyboardButton.WithUrl("Donation Alerts", "https://www.donationalerts.com/r/justvpn")
                //        }
            });
            return kbrd;
        }

        internal static IReplyMarkup? GetAdminResponseMenu(long chatId)
        {
            var kbrd = new InlineKeyboardMarkup(new InlineKeyboardButton[][]
            {
                new []  {
                        InlineKeyboardButton.WithCallbackData("Ответить на сообщение", chatId.ToString())
                        },
            });
            return kbrd;
        }

        internal static IReplyMarkup? GetCheckBlockMenu(long chatId)
        {
            var kbrd = new InlineKeyboardMarkup(new InlineKeyboardButton[][]
            {
                new []  {
                        InlineKeyboardButton.WithCallbackData("Чек не подходит", chatId.ToString())
                        },
            });
            return kbrd;
        }

        internal static IReplyMarkup? GetUnblockMenu(string username, long chatId)
        {
            var kbrd = new InlineKeyboardMarkup(new InlineKeyboardButton[][]
            {
                new []  {
                        InlineKeyboardButton.WithCallbackData(username, "Unblock")
                        }
            });
            return kbrd;
        }

        internal static IReplyMarkup GetAdminMunu()
        {
            var kbrd = new ReplyKeyboardMarkup(
                new[] {
                    new[] // row 1
                    {
                        new KeyboardButton("Общее сообщение 💬"),
                        new KeyboardButton("Написать пользователю ✍️")
                    },
                    new[] // row 2
                    {
                        new KeyboardButton("Разблокировать/заблокировать 🔐"),
                        new KeyboardButton("Поставить время всем ⏰")
                    },
                    new[] // row 3
                    {
                        new KeyboardButton("Главное меню  ↩️")
                    },
                }
                )
            {
                ResizeKeyboard = true
            };
            return kbrd; ;
        }

        internal static IReplyMarkup? GetCallBackMenuItems(List<List<CommandCallBackItem>> callBackItemLines)
        {
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
            foreach(var callBackItemLine in callBackItemLines)
            {
                var line = new List<InlineKeyboardButton>();
                foreach (var callBackItem in callBackItemLine)
                {
                    string json = JsonConvert.SerializeObject(callBackItem.CallBackData);
                    line.Add(InlineKeyboardButton.WithCallbackData(callBackItem.Name, json));
                }
                buttons.Add(line);
            }
            var kbrd = new InlineKeyboardMarkup(buttons);
            return kbrd;
        }

        internal static IReplyMarkup? AddCallBackMenuItems(InlineKeyboardMarkup keyboard, List<CommandCallBackItem> callBackItemsLine)
        {
            var lines = keyboard.InlineKeyboard.ToList();
            var line = new List<InlineKeyboardButton>();
            foreach (var callBackItem in callBackItemsLine)
            {
                string json = JsonConvert.SerializeObject(callBackItem.CallBackData);
                line.Add(InlineKeyboardButton.WithCallbackData(callBackItem.Name, json));
            }
            lines.Add(line);
            return new InlineKeyboardMarkup(lines); ;
        }
    }
}
