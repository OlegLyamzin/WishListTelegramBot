using Telegram.Bot.Types;

namespace WishListTelegramBot.BL.Models.Commands
{
    public interface ICommand
    {
        public Task Execute(Update update);
        public bool Contains(string command);
    }
}
