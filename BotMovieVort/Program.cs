using BotMovieVort.Model;
using BotMovieVort.Service;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotCosmetics.Service;

public static class Program
{
    private static TelegramBotClient Bot;
    static async Task Main(string[] args)
    {
        Bot = new TelegramBotClient(Configuration.BotToken);

        User me = await Bot.GetMeAsync();

        using var cts = new CancellationTokenSource();

        ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
        Bot.StartReceiving(Handlers.HandleUpdateAsync,
                           Handlers.HandleErrorAsync,
                           receiverOptions,
                           cts.Token);


        Console.WriteLine($"Start listening for @{me.Username}");

        while (true)
        {
            var cons = Console.ReadLine();
            if (cons == "updatefilm")
            {
                Console.WriteLine("Start update film");
                SeleniumService seleniumService = new SeleniumService();
                seleniumService.ContentFilm();
                Console.WriteLine("End update film");
            }
            if (cons == "td")
            {
                Console.WriteLine("Start td");
               TdApiServer apiServer = new TdApiServer();
                TdApiServer.Start();
                TdApiServer.getFileId(@"C:\Users\ilega\OneDrive\Рабочий стол\ConsoleApp1\ConsoleApp1\Movie\123.mp4", "m", Guid.NewGuid());
            }
            if (cons == "updateserial")
            {
                Console.WriteLine("Start update serial");
                SeleniumService seleniumService = new SeleniumService();
                seleniumService.ContentSerial();
                Console.WriteLine("End update serial");
            }
            if (cons == "exit")
            {
                cts.Cancel();
                return;
            }
        }

    }
}