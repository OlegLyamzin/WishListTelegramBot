using Telegram.Bot.Types;
using Telegram.Bot;

namespace WishListTelegramBot.BL.Models.Commands
{
    public abstract class Command
    {
        public TelegramBotClient Client { get; set; }
        public abstract List<string> Name { get;}
        public abstract Task Execute(Update update);
        public Bot Bot { get; set; }

        public Command(Bot bot)
        {
            Bot = bot;
            Client =  bot.Get();
        }
        public bool Contains(string command)
        {
            return this.Name.Contains(command);
        }
    }
}
