using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Shadowboxer;

class Program
{
    // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
    private static ITelegramBotClient _botClient;

    // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
    private static ReceiverOptions _receiverOptions;

    static async Task Main()
    {

        var env_tg_token = Environment.GetEnvironmentVariable("TG_TOKEN");

        if (env_tg_token is null)
        {
            throw new Exception("Environment variable $TG_TOKEN is empty");
        }

        _botClient = new TelegramBotClient(env_tg_token); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
        _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
        {
            AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
            {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
            },
            // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
            // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
            ThrowPendingUpdates = true,
        };

        using var cts = new CancellationTokenSource();


        using (ApplicationContext db = new ApplicationContext())
        {
            db.Users.Add(new Shadowboxer.Models.User(1740508533));
            db.SaveChanges();
        }
        using (ApplicationContext db = new ApplicationContext())
        {
            var users = db.Users.ToList();

            foreach (var user in users)
            {
                Console.WriteLine(user.Id);
            }
        }
        //// UpdateHander - обработчик приходящих Update`ов
        //// ErrorHandler - обработчик ошибок, связанных с Bot API
        //_botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

        //var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
        //Console.WriteLine($"{me.FirstName} запущен!");

        //await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
    }
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
        try
        {
            // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        // эта переменная будет содержать в себе все связанное с сообщениями
                        var message = update.Message;
                        if (message is null) return;
                        // From - это от кого пришло сообщение (или любой другой Update)
                        var user = message.From;
                        if (user is null) return;
                        // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                        Console.WriteLine($"{user.FirstName} @{user.Username} id={user.Id} написал сообщение: {message.Text}");

                        // Chat - содержит всю информацию о чате
                        var chat = message.Chat;

                        //await botClient.SendTextMessageAsync(
                        //    chat.Id,
                        //    "Русич в чате",
                        //    replyToMessageId: message.MessageId
                        //    );
                        if (message.Text is string text && text.Contains("русич", StringComparison.CurrentCultureIgnoreCase))
                            await botClient.SendAnimationAsync(chatId: chat.Id, animation: InputFile.FromUri("https://i.imgur.com/WoZkvmu.mp4"));
                        return;
                    }
            }
        }
        catch (Exception ex1)
        {
            try
            {
                Console.WriteLine(ex1.ToString());
                if (update.Message is not null)
                {
                    await botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        ex1.Message,
                        replyToMessageId: update.Message.MessageId
                    );
                }
            }
            catch (Exception ex2)
            {
                Console.WriteLine("Exception in catch block!!!");
                Console.WriteLine(ex2.ToString());
            }
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}