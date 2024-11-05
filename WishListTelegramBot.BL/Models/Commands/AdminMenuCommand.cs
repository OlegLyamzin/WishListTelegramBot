using WishListTelegramBot.BL.Models;
using WishListTelegramBot.BL;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace WishListTelegramBot.BL.Models.Commands
{
    public class AdminMenuCommand : Command, ICommand
    {
        public AdminMenuCommand(Bot bot) : base(bot)
        {
        }

        public override List<string> Name => new List<string> { "/admin" };


        public override async Task Execute(Update update)
        {
            long chatId = update.Message.Chat.Id;
            if (!Bot.IsAdmin(chatId.ToString()))
            {
                await Client.SendTextMessageAsync(chatId, "У вас нет доступа");
                return;
            }
            await Client.SendTextMessageAsync(Bot.GetMainAdmin(), "🤡 Админка 🤡", replyMarkup: Keyboards.GetAdminMunu());
        }

    }
}

