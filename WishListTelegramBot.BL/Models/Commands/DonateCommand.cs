
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WishListTelegramBot.BL.Models.Commands
{
    public class DonateCommand : Command, ICommand
    {
        public DonateCommand(Bot bot) : base(bot)
        {
        }

        public override List<string> Name => new List<string> { "Пожертвовать ❤️" };


        public override async Task Execute(Update update)
        {
            long chatId = update.Message.Chat.Id;
            await Client.SendTextMessageAsync(Bot.GetMainAdmin(), $"{update.Message.Chat.Id} - {update.Message.Chat.Username} - {update.Message.Chat.FirstName} - {update.Message.Chat.LastName} нажал пожертвовать");
            await Client.SendTextMessageAsync(chatId, @"Надеюсь, что это письмо найдет вас в благополучии! 💌🙏🌐

Благодаря вашей поддержке, мы можем сохранить наш сервис бесплатным для всех. 🤝💰🆓 

Ссылки для пожертвований 👇 ", replyMarkup: Keyboards.GetDonateInfoBoard());
        }

    }
}