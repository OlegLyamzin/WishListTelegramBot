using WishListTelegramBot.BL;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WishListTelegramBot.BL.Models.Commands
{
    public class SendMessageToAdminCommand : Command, ICommand, IListener
    {
        public override List<string> Name => new List<string> { "Написать админу 🤡" };


        public CommandExecutor Executor { get; }

        public SendMessageToAdminCommand(Bot bot, CommandExecutor executor) : base(bot)
        {
            Executor = executor;
        }


        public async override Task Execute(Update update)
        {
            long chatId = update.Message.Chat.Id;
            Executor.StartListen(this); //говорим, что теперь нам надо отправлять апдейты
            await Client.SendTextMessageAsync(chatId, "Введите сообщение (для отмены нажмите /exit)");

        }

        public async Task GetUpdate(Update update)
        {
            Executor.StopListen();

            long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;
            if (update?.Message?.Text != null && update.Message.Text == "/exit") //Проверочка{
            {
                return;
            }
            var messgae = update.Message != null ? update.Message : update.CallbackQuery.Message;
            var keayboard = Keyboards.GetCallBackMenuItems(new List<List<CommandCallBackItem>> { new List<CommandCallBackItem> { new CommandCallBackItem { Name = "Ответить на сообщение", CallBackData = new CallBackData { Command = "/replymessage", Data = chatId.ToString() } } } });
            await Client.SendTextMessageAsync(Bot.GetMainAdmin(), $" {chatId} - @{messgae.Chat.Username} - {messgae.Chat.FirstName} - {messgae.Chat.LastName} - Пишет: {messgae.Text} ", replyMarkup: keayboard);

            switch (messgae.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Sticker:
                    await Client.SendStickerAsync(Bot.GetMainAdmin(), sticker: new InputFileId(messgae.Sticker.FileId));
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Photo:
                    await Client.SendPhotoAsync(Bot.GetMainAdmin(), new InputFileId(messgae.Photo.Last().FileId));
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Video:
                    await Client.SendVideoAsync(Bot.GetMainAdmin(), new InputFileId(messgae.Video.FileId));
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Voice:
                    await Client.SendVoiceAsync(Bot.GetMainAdmin(), new InputFileId(messgae.Voice.FileId));
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Text:
                    break;
                default:
                    await Client.SendTextMessageAsync(Bot.GetMainAdmin(), "Другой формат сообщения " + messgae.Type.ToString());
                    break;
            }



        }
    }
}
