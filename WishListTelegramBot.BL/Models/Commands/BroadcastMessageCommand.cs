using WishListTelegramBot.BL;
using WishListTelegramBot.BL.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WishListTelegramBot.BL.Models.Commands
{
    public class BroadcastMessageCommand : BuisnessLogicCommand, IListener, ICommand
    {
        public override List<string> Name => new List<string> { "/broadcast", "Общее сообщение 💬" };


        public CommandExecutor Executor { get; }

        public BroadcastMessageCommand(Bot bot, CommandExecutor executor, DataBaseConnector serverConnector) : base(bot, serverConnector)
        {
            Executor = executor;
        }


        public async override Task Execute(Update update)
        {
            long chatId = update.Message.Chat.Id;
            if (!Bot.IsAdmin(chatId.ToString()))
            {
                await Client.SendTextMessageAsync(chatId, "У вас нет доступа");
                return;
            }
            Executor.StartListen(this); //говорим, что теперь нам надо отправлять апдейты
            await Client.SendTextMessageAsync(chatId, "Введите сообщение (для отмены нажмите /exit)");

        }

        public async Task GetUpdate(Update update)
        {
            Executor.StopListen();
            long chatId = update.Message.Chat.Id;
            if (update.Message.Text == null || update.Message.Text == "/exit") //Проверочка{
            {
                return;
            }

            List<DL.Models.User> users = await DataBaseConnector.UserService.GetAllUsersAsync();
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.TgId))
                {
                    try
                    {
                        await Client.SendTextMessageAsync(user.TgId, update.Message.Text, replyMarkup: Keyboards.GetMainMenuBoard(Bot.IsAdmin(user.TgId.ToString())));
                    }
                    catch
                    {
                    }
                }

            }

        }

    }
}
