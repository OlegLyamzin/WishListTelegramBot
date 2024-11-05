using WishListTelegramBot.BL.Models.Commands;
using WishListTelegramBot.Core;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace WishListTelegramBot.BL.Models
{
    public class Bot
    {
        private TelegramBotClient client;
        private List<Command> commandList;
        private string adminId;
        private string supervisorId;
        private string url;

        public IReadOnlyList<Command> Commands { get => commandList.AsReadOnly(); }

        public Bot(IOptions<AppSettings> option)
        {
            Init(option.Value.API_KEY, option.Value.URL);
            adminId = option.Value.ADMINID;
            supervisorId = option.Value.SUPERVISORID;
            url = option.Value.URL;
        }

        public TelegramBotClient Get()
        {
            return client;
        }

        public TelegramBotClient Init(string apiKey, string url)
        {
            commandList = new List<Command>();

            client = new TelegramBotClient(apiKey);
            client.SetWebhookAsync(url).Wait();
            return client;
        }

        public bool IsAdmin(string id)
        {
            return id == adminId;
        }

        public string GetMainAdmin()
        {
            return adminId;
        }
        public string[] GetSupervisor()
        {
            return supervisorId.Split(",");
        }

        public string GetURL()
        {
            return url;
        }
    }
}
