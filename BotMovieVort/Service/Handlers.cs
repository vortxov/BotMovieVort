
using BotMovieVort.Domain;
using BotMovieVort.Model;
using BotMovieVort.Service;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotCosmetics.Service;

public class Handlers
{
    public static DataManager dataManager = new DataManager();


    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) 
    {
        var ErrorMessage = exception switch 
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask; 
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
            var handler = update.Type switch  
        {
            UpdateType.Message => BotOnMessageReceived(botClient, update.Message!), 
            UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
            UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!), 
            UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery!), 
            UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult!),                                                                                                      
        _ => UnknownUpdateHandlerAsync(botClient, update)
        };

        try
        {
            await handler; 
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(botClient, exception, cancellationToken); 
        }
    }

    private static async Task<Message> BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
    {
        return null;
    }

    private static async Task<Message> BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        try
        {
            var data = callbackQuery.Data.Split("/");
            if (data[0] == "m")
            {
                var film = dataManager.itemFilm.GetItemFilmById(Guid.Parse(data[1])).Result;

                return await botClient.SendVideoAsync(
                       chatId: callbackQuery.Message.Chat.Id,
                       video: new InputOnlineFile(film.FileId),
                       caption: film.Name,
                       parseMode: ParseMode.Html,
                       supportsStreaming: true);
            }
            else if(data[0] == "Cgf")
            {
                return await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: dataManager.genre.GetGenreById(Guid.Parse(data[1])).Result.Name + ":",
                        replyMarkup: SearchItems(data[1], int.Parse(data[2]), "Cgf"));
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception();
        }


        throw new NotImplementedException();
    }

    private static async Task<Message> SendMessageMovie(ITelegramBotClient botClient, CallbackQuery callbackQuery, Guid id)
    {
        var film = dataManager.itemFilm.GetItemFilmById(id).Result;

        using (var stream = System.IO.File.OpenRead(film.Path))
        {
            return await botClient.SendVideoAsync(chatId: callbackQuery.Message.Chat.Id,
                                                    video: stream);
        }
    }


    

    private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
    {
        Console.WriteLine($"Receive message type: {message.Type}");
        if (message.Type == MessageType.Text)
        {

            var action = message.Text switch
            {
                "/start" => SendStartKeyboard(botClient, message),
                "Поиск" => SendStartKeyboard(botClient, message),
                "Фильмы" => SendFilmKeyboard(botClient, message),
                "Назад" => SendStartKeyboard(botClient, message),
                "Список всех фильмов" => SendAllFilm(botClient, message),
                "По жанрам" => SendGenresFilm(botClient, message),
                _ => SendSearchItem(botClient, message)
            };
            Message sentMessage = await action;
            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}, text: {message.Text}");
        }
        else if(message.Type == MessageType.Video)
        {
            var action = message.Caption.Split(":")[0] switch
            {
                "/m" => GetFilmId(botClient, message),
                "/s" => GetSeriesId(botClient, message)
            };
        }
        return;
    }

    private static async Task GetFilmId(ITelegramBotClient botClient, Message message)
    {
        var film = dataManager.itemFilm.GetItemFilmById(Guid.Parse(message.Caption.Split(":")[1])).Result;
        if (film == null)
            return;
        film.FileId = message.Video.FileId;

        await dataManager.itemFilm.UpdateItemFilm(film);
    }

    private static async Task GetSeriesId(ITelegramBotClient botClient, Message message)
    {
        var series = dataManager.series.GetSeriesById(Guid.Parse(message.Text.Split(":")[1])).Result;
        if (series == null)
            return;
        series.FileId = message.Video.FileId;

        await dataManager.series.UpdateSeries(series);
    }

    private static async Task<Message> SendStartKeyboard(ITelegramBotClient botClient, Message message)
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new(  
                new[]
                {
                        new KeyboardButton[] { "Поиск", "Случайное" },
                        new KeyboardButton[] { "Фильмы", "Сериалы" },
                        new KeyboardButton[] { "Инструкция" }
                })
        {
            ResizeKeyboard = true
        };

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Теперь вы можете приступить к просмотру сериалов.\nВоспользуйтесь кнопками ниже или напишите боту название нужного фильма/сериала!👇",
                                                    replyMarkup: replyKeyboardMarkup);
    }


    private static async Task<Message> SendFilmKeyboard(ITelegramBotClient botClient, Message message)
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                        new KeyboardButton[] { "Список всех фильмов", "По жанрам" },
                        new KeyboardButton[] { "Рекомендуемые", "Топ 100" },
                        new KeyboardButton[] { "Назад" }
                })
        {
            ResizeKeyboard = true
        };

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Фильмы:",
                                                    replyMarkup: replyKeyboardMarkup);
    }

    private static async Task<Message> SendGenresFilm(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboard = SearchItems(message.Text, 0, "gf");

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Ниже представлен список всех фильмов",
                                                    replyMarkup: inlineKeyboard);
    }


    private static async Task<Message> SendAllFilm(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboard = SearchItems(message.Text, 0, "af");

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Ниже представлен список всех фильмов",
                                                    replyMarkup: inlineKeyboard);
    }

    private static async Task<Message> SendSearchItem(ITelegramBotClient botClient, Message message)
    {
        if (message.Text.Length < 3)
        {
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Введеное название некоректно");
        }

        var inlineKeyboard = SearchItems(message.Text, 0, "ms");

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Ниже представлен список по запросу <<" + message.Text + ">>",
                                                    replyMarkup: inlineKeyboard);
    }

    private static InlineKeyboardMarkup SearchItems(string message, int page, string type)
    {
        List<Items> items = new List<Items>();
        if (type == "ms")
        {
            var films = dataManager.itemFilm.GetItemFilms().Result;

            foreach (var film in films)
            {
                if (film.Name.ToLower().Contains(message.ToLower()))
                {
                    items.Add(new Items() { Text = film.Name + " (" + film.Year + ")", Data = "m/" + film.Id });
                }
            }

            var serials = dataManager.itemSerials.GetItemSerials().Result;

            foreach (var serial in serials)
            {
                if (serial.Name.ToLower().Contains(message.ToLower()))
                {
                    items.Add(new Items() { Text = serial.Name, Data = "s/" + serial.Id });
                }
            }

            return ViewItems(items, message, type, page);
        }
        else if(type == "af")
        {
            var films = dataManager.itemFilm.GetItemFilms().Result;

            foreach (var film in films)
            {
                items.Add(new Items() { Text = film.Name + " (" + film.Year + ")", Data = "m/" + film.Id });
            }

            return ViewItems(items, message, type, page);
        }
        else if (type == "gf")
        {
            var genres = dataManager.genre.GetGenres().Result;

            foreach (var genre in genres)
            {
                items.Add(new Items() { Text = genre.Name, Data = "Cgf/" + genre.Id + "/0" });
            }

            return ViewItems(items, message, type, page);
        }
        else if (type == "Cgf")
        {
            var films = dataManager.genre.GetGenreById(Guid.Parse(message)).Result.ItemFilms;

            foreach (var film in films)
            {
                items.Add(new Items() { Text = film.Name + " (" + film.Year + ")", Data = "m/" + film.Id });
            }

            return ViewItems(items, message, type, page);
        }
        return null;
    }

    private static InlineKeyboardMarkup ViewItems(List<Items> items, string message, string type, int page)
    {
        

        List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

        for (int i = 0 + (page * 10); i < items.Count && i < (page + 1) * 10; i++)
        {
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData(items[i].Text, items[i].Data) });
        }

        if (page == 0 && items.Count > 10)
        {
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("-->", type + "/" + message + "/" + (page + 1)) });
        }
        else if (items.Count - page * 10 > 10)
        {
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", type + "/" + message + "/" + (page - 1)) });
        }
        else if (items.Count > 10 && page > 1)
        {
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", type + "/" + message + "/" + (page - 1)),
                                    InlineKeyboardButton.WithCallbackData("-->", type + "/" + message + "/" + (page + 1))});
        }

        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

        return inlineKeyboard;
    }

    private static Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult) 
    {
        Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
        return Task.CompletedTask;
    }

    private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update) 
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}
