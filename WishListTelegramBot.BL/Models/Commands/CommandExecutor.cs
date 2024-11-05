
using WishListTelegramBot.BL;
using System.Windows.Input;
using Telegram.Bot.Types;
using System.Text.Json;

namespace WishListTelegramBot.BL.Models.Commands
{
    public class CommandExecutor
    {
        private List<ICommand> commands;
        private IListener? listener = null;
        private Bot _bot;
        private DataBaseConnector _dataBaseConnector;

        public CommandExecutor(Bot bot, DataBaseConnector dataBaseConnector)
        {
            _bot = bot;
            _dataBaseConnector = dataBaseConnector;
            commands = GetCommands(bot);
        }
        private List<ICommand> GetCommands(Bot bot)
        {
            var types = AppDomain
                      .CurrentDomain
                      .GetAssemblies()
                      .SelectMany(assembly => assembly.GetTypes())
                      .Where(type => typeof(ICommand).IsAssignableFrom(type))
                      .Where(type => type.IsClass);

            List<ICommand> commands = new List<ICommand>();
            foreach (var type in types)
            {
                ICommand? command;
                List<object> paramsConstructor = new List<object> { bot };
                if (typeof(IListener).IsAssignableFrom(type))
                {
                    paramsConstructor.Add(this);
                }
                if (typeof(BuisnessLogicCommand).IsAssignableFrom(type))
                {
                    paramsConstructor.Add(_dataBaseConnector);
                }
                command = Activator.CreateInstance(type, paramsConstructor.ToArray()) as ICommand;

                if (command != null)
                {
                    commands.Add(command);
                }
            }
            return commands;
        }

        public async Task GetUpdate(Update update)
        {

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                await ExecuteCommand(update);
            }

            if (update.Message == null) //и такое тоже бывает, делаем проверку
                return;

            if (listener == null)
            {
                await ExecuteCommand(update);
            }
            else
            {
                await listener.GetUpdate(update);
            }
        }

        private async Task ExecuteCommand(Update update)
        {
            try
            {
                string msgText = update?.Message?.Text;
                if (update.Message == null && update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && !string.IsNullOrEmpty(update.CallbackQuery.Data))
                {
                    CallBackData? callBackData = JsonSerializer.Deserialize<CallBackData>(update.CallbackQuery.Data);
                    msgText = callBackData.Command;
                }
                if (string.IsNullOrEmpty(msgText))
                {
                    return;
                }
                if (msgText.Contains("/start"))
                    msgText = "/start";
                bool finded = false;
                foreach (var command in commands)
                {
                    if (command.Contains(msgText))
                    {
                        finded = true;
                        await command.Execute(update);
                    }
                }
                if (!finded)
                {
                    await new SendMessageToAdminCommand(_bot, this).GetUpdate(update);
                }
            }
            catch (Exception ex)
            {
                await new SendMessageToAdminCommand(_bot, this).GetUpdate(update);
                throw ex;
            }
        }

        public void StartListen(IListener newListener)
        {
            listener = newListener;
        }

        public void StopListen()
        {
            listener = null;
        }
    }
}
