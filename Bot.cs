using PrintManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace PrinterTgBot
{
    /// <summary>
    /// The bot class. Within this class doing configuration and work of the bot.
    /// </summary>
    internal class Bot
    {
        private static string Token=YOUR API TOKEN";
        TelegramBotClient botClient;
        Telegram.Bot.Types.User me;
        ReplyKeyboardMarkup replyKeyboard;
        ReceiverOptions receiverOptions;
        CancellationToken cancellationToken;
        FileManager fileManager = new FileManager();       
        Printer printer = new Printer();

        public Bot()
        {
            var rec = new ReceiverOptions();

            botClient = new TelegramBotClient(Token);
            Task.Run(async () => { me = await botClient.GetMeAsync(); });

            replyKeyboard = new ReplyKeyboardMarkup(new List<KeyboardButton[]>()
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Запустить")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Распечатать файл")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Зарегистрироваться (пока не работает)")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Помощь"),
                    new KeyboardButton("Остановить (пока не работает)")
                },
            });
            cancellationToken = new CancellationToken();
            if (!System.IO.File.Exists(fileManager.DestinationDirectory + '\\' + "defaultPrinter.json"))
                SerializePrinter();
             
            DeserializePrinter();

        }

        private void DeserializePrinter()
        {
            string fileName = fileManager.DestinationDirectory + '\\' + "defaultPrinter.json";
            string jsonString = System.IO.File.ReadAllText(fileName);
            printer = JsonSerializer.Deserialize<Printer>(jsonString)!;
        }

        /// <summary>
        /// This method starts the bot
        /// </summary>
        public void StartReceiving()
        {
            receiverOptions= new ReceiverOptions
            {
                AllowedUpdates = new[] // Types of processing types of updates
                {
                UpdateType.Message, // Text, Photo/Video, voice/video messages etc.
                },
                ThrowPendingUpdates = true //True - bot doesn't process those messages which were sended when bot was offline
                                           //False(by default) - bot processes those messages which were sended when bot was offline
                
            };
            botClient.StartReceiving(Update, Error,receiverOptions,cancellationToken);
            
        }

        private async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            await Console.Out.WriteLineAsync(exception.Message);
            throw new NotImplementedException();
        }
        private void SerializePrinter()
        {
            string fileName = fileManager.DestinationDirectory+'\\'+"defaultPrinter.json";
            string jsonString = JsonSerializer.Serialize(printer);
            System.IO.File.WriteAllText(fileName, jsonString);
        }
        /// <summary>
        /// This method processes comand etc. It is called from StartReceiving method.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="update">The update.</param>
        /// <param name="token">The token.</param>
        /// <returns>A Task.</returns>
        private async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            try
            {
                var message = update.Message;
                if (message == null) return;
                await Console.Out.WriteLineAsync($"Пользователь {message.Chat.Id} отправил сообщение");

                if (message.Text!=null)
                {
                    var textMessage = message.Text.Trim().ToLower();
                    switch (textMessage)
                    {
                        case "/start":
                        case "запустить":
                            await client.SendTextMessageAsync(message.Chat.Id, "Привет! Я - бот для печати твоих файлов.Выбери, " +
                                "что ты хочешь на клавиатуре снизу.", 
                                replyMarkup: replyKeyboard,cancellationToken: cancellationToken);
                            break;
                        case "/stop":
                        case "остановить":
                            await client.SendTextMessageAsync(message.Chat.Id, "Пока!",replyMarkup: replyKeyboard, cancellationToken: cancellationToken);
                            break;
                        case "/print_file":
                        case "распечатать файл":
                            await client.SendTextMessageAsync(message.Chat.Id, "Отправь pdf-файл. Пока что я умею печатать файлы " +
                                "размером до 20 Мб.");
                            
                            break;
                        case "/register":
                        case "зарегистрироваться":
                            await client.SendTextMessageAsync(message.Chat.Id, "Окей, когда администратор тебя зарегистрирует, я дам тебе знать!", replyMarkup: replyKeyboard, cancellationToken: cancellationToken);
                            break;
                        case "/help":
                        case "помощь":
                            await client.SendTextMessageAsync(message.Chat.Id, "Список команд:\n" +
                                "/start - запустить бот\n/stop - остановить бот\n/print_file - распечатать файл\n/register - зарегистрироваться\n /help - помощь"
                                , replyMarkup: replyKeyboard, cancellationToken: cancellationToken);
                            break;
                        default:
                            await client.SendTextMessageAsync(message.Chat.Id, "Я не знаю такую команду! Попробуй ещё, вот список команд:\n" +
                                "/start\n/stop\n/print_file\n/register", replyMarkup: replyKeyboard, cancellationToken: cancellationToken);
                            break;
                    }

                }
                if (message.Document != null)
                {

                    var fileId = message.Document.FileId;
                    var fileInfo = await botClient.GetFileAsync(fileId);
                    var filePath=fileInfo.FilePath;
                    
                    var filename = fileManager.GetDestinationPath(message.Document.FileName);
                    await using var fileStream = System.IO.File.OpenWrite(filename);
                    await client.SendTextMessageAsync(message.Chat.Id, "Я получил файл, печатаю...",
                        replyMarkup: replyKeyboard, cancellationToken: cancellationToken);
                    await botClient.DownloadFileAsync(filePath,fileStream);
                    fileStream.Close();
                    
                    await printer.PrintAsync(filename);

                    await client.SendTextMessageAsync(message.Chat.Id, "Файл распечатан!",
                        replyMarkup: replyKeyboard, cancellationToken: cancellationToken);
                }
                
               

                return;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }

        }
    }
}
