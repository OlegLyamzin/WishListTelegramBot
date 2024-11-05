using Telegram.Bot.Types;

namespace WishListTelegramBot.BL.Models.Commands
{
    public interface IListener
    {
        public Task GetUpdate(Update update);

        public CommandExecutor Executor { get; }
    }
}
