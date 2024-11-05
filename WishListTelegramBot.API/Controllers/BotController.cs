
using DocumentFormat.OpenXml.Office2013.Word;
using WishListTelegramBot.BL;
using WishListTelegramBot.BL.Models;
using WishListTelegramBot.Core;
using WishListTelegramBot.DL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Runtime;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WishListTelegramBot.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class BotController : ControllerBase
    {
        private Bot _bot;
        private UpdateDistributor _updateDistributor;
        private AppSettings _appSettings;
        private DataBaseConnector _dataBaseConnector;

        public BotController( Bot bot, DataBaseConnector dataBaseConnector, UpdateDistributor updateDistributor, IOptions<AppSettings> options) {

            _bot = bot;
            _updateDistributor = updateDistributor;
            _appSettings = options.Value;
            _dataBaseConnector = dataBaseConnector;
        }
        [HttpPost]
        public async void Post(Update update) //Сюда будут приходить апдейты
        {
            try
            {       
                await _updateDistributor.GetUpdate(update);
            }            
            catch(Exception ex)
            {
                try
                {
                    var Client = _bot.Get();
                    if (update.Message != null)
                    {
                        await Client.SendTextMessageAsync(_bot.GetMainAdmin(), $" {update.Message.Chat.Id} - @{update.Message.Chat.Username} - {update.Message.Chat.FirstName} - {update.Message.Chat.LastName} - {ex.Message}");
                        await Client.SendTextMessageAsync(update.Message.Chat.Id, "Что-то пошло не так");
                    }
                    else if (update.CallbackQuery.Message != null)
                    {
                        await Client.SendTextMessageAsync(_bot.GetMainAdmin(), $" {update.CallbackQuery.Message.Chat.Id} - @{update.CallbackQuery.Message.Chat.Username} - {update.CallbackQuery.Message.Chat.FirstName} - {update.CallbackQuery.Message.Chat.LastName} - {ex.Message}");
                        await Client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Что-то пошло не так");
                    }
                    else
                    {
                        await Client.SendTextMessageAsync(_bot.GetMainAdmin(), $"{ex.Message}");
                    }
                }
                catch { }

            }
        }
        [HttpGet]
        public async Task<string> Get()
        {
            //Здесь мы пишем, что будет видно если зайти на адрес,
            //указаную в ngrok и launchSettings
            try
            {
            var Client = _bot.Get();
            var info = await Client.GetWebhookInfoAsync();
            return "Telegram bot was started " + _appSettings.URL + " " + info.Url + " " + info.LastErrorMessage;
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }
    }
}
