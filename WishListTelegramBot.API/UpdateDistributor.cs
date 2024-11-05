
using WishListTelegramBot.BL;
using WishListTelegramBot.BL.Models;
using WishListTelegramBot.BL.Models.Commands;
using Telegram.Bot.Types;

namespace WishListTelegramBot.API
{
    public class UpdateDistributor
    {
        private Dictionary<long, CommandExecutor> listeners;
        private Bot _bot;
        private DataBaseConnector _dataBaseConnector;
        public UpdateDistributor(Bot bot, DataBaseConnector dataBaseConnector)
        {
            listeners = new Dictionary<long, CommandExecutor>();
            _bot = bot;
            _dataBaseConnector = dataBaseConnector;
        }

        public async Task GetUpdate(Update update)
        {
            
            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;
            CommandExecutor listener = listeners.GetValueOrDefault(chatId);
            if (listener is null)
            {
                listener = new CommandExecutor(_bot, _dataBaseConnector);
                listeners.Add(chatId, listener);
                await listener.GetUpdate(update);
                return;
            }
            await listener.GetUpdate(update);
        }
    }
}
