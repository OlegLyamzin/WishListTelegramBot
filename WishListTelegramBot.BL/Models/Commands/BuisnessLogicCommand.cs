using WishListTelegramBot.BL;
using Telegram.Bot.Types;

namespace WishListTelegramBot.BL.Models.Commands
{
    public abstract class BuisnessLogicCommand : Command
    {
        protected DataBaseConnector DataBaseConnector { get; set; }
        protected BuisnessLogicCommand(Bot bot, DataBaseConnector dataBaseConnector) : base(bot)
        {
            this.DataBaseConnector = dataBaseConnector;
        }

        public BuisnessLogicCommand(Bot bot) : base(bot)
        {
        }
    }
}