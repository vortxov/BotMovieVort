
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
            if (data[0] == "st")
            {
                var series = dataManager.series.GetSeriesById(Guid.Parse(data[1])).Result;
                var caption = series.Season.Serials.Name + " (" + series.Season.Serials.Year + ")\n" +
                              series.Season.Number + " сезон\n" +
                              series.NumberSeries + " из " + series.Season.Series.Count + " серий";

                List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

                if (data.Count() > 2)
                {
                    if (series.NumberSeries == 1)
                    {
                        buttons.Add(new[] {InlineKeyboardButton.WithCallbackData("К сезонам", "ssa/" +
                                             series.Season.Serials.Id
                                            + "/" + data[2] + "/" + data[3]),
                                        InlineKeyboardButton.WithCallbackData("След", "st/" +
                                               series.Season.Series[series.NumberSeries].Id.ToString()
                                               + "/" + data[2] + "/" + data[3]),});
                    }
                    else if (series.NumberSeries == series.Season.Series.Count)
                    {
                        buttons.Add(new[] {InlineKeyboardButton.WithCallbackData("Пред", "st/" +
                                             series.Season.Series[series.NumberSeries - 2].Id.ToString()
                                             + "/" + data[2] + "/" + data[3]),
                                        InlineKeyboardButton.WithCallbackData("К сезонам", "ssa/" +
                                             series.Season.Serials.Id
                                            + "/" + data[2] + "/" + data[3]),});
                    }
                    else
                    {
                        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Пред", "st/" +
                                                 series.Season.Series[series.NumberSeries - 2].Id.ToString()
                                                 + "/" + data[2] + "/" + data[3]),
                                        InlineKeyboardButton.WithCallbackData("След", "st/" +
                                                 series.Season.Series[series.NumberSeries].Id.ToString()
                                                 + "/" + data[2] + "/" + data[3])});
                    }
                }
                else
                {
                    if (series.NumberSeries == 1)
                    {
                        buttons.Add(new[] {InlineKeyboardButton.WithCallbackData("К сезонам", "ssa/" +
                                             series.Season.Serials.Id),
                                        InlineKeyboardButton.WithCallbackData("След", "st/" +
                                               series.Season.Series[series.NumberSeries].Id.ToString()),});
                    }
                    else if (series.NumberSeries == series.Season.Series.Count)
                    {
                        buttons.Add(new[] {InlineKeyboardButton.WithCallbackData("Пред", "st/" +
                                             series.Season.Series[series.NumberSeries - 2].Id.ToString()),
                                        InlineKeyboardButton.WithCallbackData("К сезонам", "ssa/" +
                                             series.Season.Serials.Id),});
                    }
                    else
                    {
                        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Пред", "st/" +
                                                 series.Season.Series[series.NumberSeries - 2].Id.ToString()),
                                        InlineKeyboardButton.WithCallbackData("След", "st/" +
                                                 series.Season.Series[series.NumberSeries].Id.ToString())});
                    }
                }


                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

                return await botClient.SendVideoAsync(
                       chatId: callbackQuery.Message.Chat.Id,
                       video: new InputOnlineFile(series.FileId),
                       caption: caption,
                       parseMode: ParseMode.Html,
                       supportsStreaming: true,
                       replyMarkup: inlineKeyboard);
            }
            else if (data[0] == "ss")
            {
                var serial = dataManager.itemSerials.GetItemSerialsById(Guid.Parse(data[1])).Result;
                string text = serial.Name + " (" + serial.Year + ")" + "\n" + "\n" +
                    "Выберите сезон:";

                List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();


                if (data.Count() > 2)
                {
                    for (int i = 0; i < serial.Seasons.Count; i++)
                    {
                        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData((i + 1).ToString(), "ssr/" + serial.Seasons[i].Id.ToString() + "/" + data[2] + "/" + data[3]) });
                    }

                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "s/" + serial.Id + "/" + data[2] + "/" + data[3]) });
                }
                else
                {
                    for (int i = 0; i < serial.Seasons.Count; i++)
                    {
                        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData((i + 1).ToString(), "ssr/" + serial.Seasons[i].Id.ToString()) });
                    }

                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "s/" + serial.Id) });
                }



                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

                return await botClient.EditMessageTextAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: text,
                        replyMarkup: inlineKeyboard);
            }
            else if (data[0] == "move")
            {
                var inlineKeyboard = SearchItems(data[2], int.Parse(data[3]), data[1]);

                return await botClient.EditMessageTextAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: data[2],
                        replyMarkup: inlineKeyboard);
            }
            else if (data[0] == "ssa")
            {
                var serial = dataManager.itemSerials.GetItemSerialsById(Guid.Parse(data[1])).Result;
                string text = serial.Name + " (" + serial.Year + ")" + "\n" + "\n" +
                    "Выберите сезон:";

                List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();


                

                if (data.Count() > 2)
                {
                    for (int i = 0; i < serial.Seasons.Count; i++)
                    {
                        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData((i + 1).ToString(), "ssr/" + serial.Seasons[i].Id.ToString() + "/" + data[2] + "/" + data[3]) });
                    }

                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "s/" + serial.Id + "/" + data[2] + "/" + data[3]) });
                }
                else
                {
                    for (int i = 0; i < serial.Seasons.Count; i++)
                    {
                        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData((i + 1).ToString(), "ssr/" + serial.Seasons[i].Id.ToString()) });
                    }

                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "s/" + serial.Id) });
                }


                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

                return await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: text,
                        replyMarkup: inlineKeyboard);
            }

            else if (data[0] == "ssr")
            {
                var season = dataManager.season.GetSeasonById(Guid.Parse(data[1])).Result;
                string text = season.Serials.Name + " (" + season.Serials.Year + ")" + "\n" +
                    "Сезон:" + season.Number + " сезон" + "\n" + "\n" +
                    "Выберите серию:";

                List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

               
                if (data.Count() > 2)
                {
                    for (int i = 0; i < season.Series.Count; i++)
                    {
                        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData((i + 1) + " серия", "st/" + season.Series[i].Id.ToString() + "/" + data[2] + "/" + data[3]) });
                    }

                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "ss/" + season.Serials.Id + "/" + data[2] + "/" + data[3]) });
                }
                else
                {
                    for (int i = 0; i < season.Series.Count; i++)
                    {
                        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData((i + 1) + " серия", "st/" + season.Series[i].Id.ToString()) });
                    }

                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", "ss/" + season.Serials.Id) });
                }


                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

                return await botClient.EditMessageTextAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: text,
                        replyMarkup: inlineKeyboard);
            }
            else if (data[0] == "s")
            {
                var serial = dataManager.itemSerials.GetItemSerialsById(Guid.Parse(data[1])).Result;
                string text = serial.Name + " (" + serial.Year + ")" + "\n" + "\n" +
                    serial.Description + "\n" + "\n" +
                    "Кинопоиск:" + serial.RatingKP + "\n" +
                    "IMDB:" + serial.RatingIMDB + "\n" + "\n" +
                    "Выберите действие из списка ниже:";

                List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();
                if (data.Count() > 2)
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Начать просмотр", "ss/" + data[1] + "/" + data[2] + "/" + data[3]) });
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад к списку сериалов", data[2] + "/" + data[3]) });
                }
                else
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Начать просмотр", "ss/" + data[1]) });
                }



                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

                return await botClient.EditMessageTextAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: text,
                        replyMarkup: inlineKeyboard);
            }

            else if (data[0] == "as")
            {
                var inlineKeyboard = SearchItems("", int.Parse(data[1]), data[0]);

                return await botClient.EditMessageTextAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: "Ниже представлен список всех сериалов",
                        replyMarkup: inlineKeyboard);
            }
            else if(data[0] == "Cgf")
            {
                return await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: dataManager.genre.GetGenreById(Guid.Parse(data[1])).Result.Name + ":",
                        replyMarkup: SearchItems(data[1], int.Parse(data[2]), "Cgf"));
            }
            else if (data[0] == "Cgs")
            {
                return await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: dataManager.genre.GetGenreById(Guid.Parse(data[1])).Result.Name + ":",
                        replyMarkup: SearchItems(data[1], int.Parse(data[2]), data[0]));
            }
            else if (data[0] == "rs")
            {
                return await botClient.EditMessageTextAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: "Ниже представлены сериалы по рекомендациям",
                        replyMarkup: SearchItems("", int.Parse(data[1]), data[0]));
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
                "Сериалы" => SendSerialKeyboard(botClient, message),
                "Назад" => SendStartKeyboard(botClient, message),
                "Список всех фильмов" => SendAllFilm(botClient, message),
                "Список всех сериалов" => SendAllSerial(botClient, message),
                "Случайный фильм" => SendRandomFilm(botClient, message),
                "Случайный сериал" => SendRandomSerial(botClient, message),
                "По жанрам фильмы" => SendGenresFilm(botClient, message),
                "По жанрам сериалы" => SendGenresSerial(botClient, message),
                "Рекомендуемые фильмы" => SendRecomFilm(botClient, message),
                "Рекомендуемые сериалы" => SendRecomSerial(botClient, message),
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
        var series = dataManager.series.GetSeriesById(Guid.Parse(message.Caption.Split(":")[1])).Result;
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
                        new KeyboardButton[] { "Список всех фильмов", "По жанрам фильмы" },
                        new KeyboardButton[] { "Рекомендуемые фильмы", "Случайный фильм" },
                        new KeyboardButton[] { "Назад" }
                })
        {
            ResizeKeyboard = true
        };

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Фильмы:",
                                                    replyMarkup: replyKeyboardMarkup);
    }

    private static async Task<Message> SendSerialKeyboard(ITelegramBotClient botClient, Message message)
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                        new KeyboardButton[] { "Список всех сериалов", "По жанрам сериалы" },
                        new KeyboardButton[] { "Рекомендуемые сериалы", "Случайный сериал" },
                        new KeyboardButton[] { "Назад" }
                })
        {
            ResizeKeyboard = true
        };

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Сериалы:",
                                                    replyMarkup: replyKeyboardMarkup);
    }

    private static async Task<Message> SendRandomFilm(ITelegramBotClient botClient, Message message)
    {
        var films = dataManager.itemFilm.GetItemFilms().Result.ToList();

        int index = new Random().Next(films.Count());

        var film = films[index];

        return await botClient.SendVideoAsync(
               chatId: message.Chat.Id,
               video: new InputOnlineFile(film.FileId),
               caption: film.Name,
               parseMode: ParseMode.Html,
               supportsStreaming: true);
    }
    private static async Task<Message> SendRandomSerial(ITelegramBotClient botClient, Message message)
    {
        var serials = dataManager.itemSerials.GetItemSerials().Result.ToList();

        int index = new Random().Next(serials.Count());

        var serial = serials[index];

        string text = serial.Name + " (" + serial.Year + ")" + "\n" + "\n" +
                    serial.Description + "\n" + "\n" +
                    "Кинопоиск:" + serial.RatingKP + "\n" +
                    "IMDB:" + serial.RatingIMDB + "\n" + "\n" +
                    "Выберите действие из списка ниже:";

        List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Начать просмотр", "ss/" + serial.Id + "/as/0") });



        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

        return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text,
                replyMarkup: inlineKeyboard);
    }

    private static async Task<Message> SendGenresFilm(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboard = SearchItems(message.Text, 0, "gf");

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Ниже представлен список жанров",
                                                    replyMarkup: inlineKeyboard);
    }

    private static async Task<Message> SendGenresSerial(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboard = SearchItems(message.Text, 0, "gs");

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Ниже представлен список жанров",
                                                    replyMarkup: inlineKeyboard);
    }


    private static async Task<Message> SendRecomFilm(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboard = SearchItems(message.Text, 0, "rf");

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Ниже представлены фильмы по рекомендациям",
                                                    replyMarkup: inlineKeyboard);
    }

    private static async Task<Message> SendRecomSerial(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboard = SearchItems(message.Text, 0, "rs");

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Ниже представлены сериалы по рекомендациям",
                                                    replyMarkup: inlineKeyboard);
    }


    private static async Task<Message> SendAllFilm(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboard = SearchItems(message.Text, 0, "af");

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Ниже представлен список всех фильмов",
                                                    replyMarkup: inlineKeyboard);
    }

    private static async Task<Message> SendAllSerial(ITelegramBotClient botClient, Message message)
    {
        var inlineKeyboard = SearchItems(message.Text, 0, "as");

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Ниже представлен список всех сериалов",
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
            var films = dataManager.itemFilm.GetItemFilms().Result.ToList();

            films.Sort((x, y) => int.Parse(y.Year).CompareTo(int.Parse(x.Year)));

            foreach (var film in films)
            {
                items.Add(new Items() { Text = film.Name + " (" + film.Year + ")", Data = "m/" + film.Id });
            }

            return ViewItems(items, message, type, page);
        }
        else if (type == "as")
        {
            var serials = dataManager.itemSerials.GetItemSerials().Result.ToList();

            serials.Sort((x, y) => int.Parse(y.Year).CompareTo(int.Parse(x.Year)));

            foreach (var serial in serials)
            {
                items.Add(new Items() { Text = serial.Name + " (" + serial.Year + ")", Data = "s/" + serial.Id + "/" + type + "/" + page  });
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
        else if (type == "gs")
        {
            var genres = dataManager.genre.GetGenres().Result;

            foreach (var genre in genres)
            {
                items.Add(new Items() { Text = genre.Name, Data = "Cgs/" + genre.Id + "/0" });
            }

            return ViewItems(items, message, type, page);
        }
        else if (type == "Cgs")
        {
            var serials = dataManager.genre.GetGenreById(Guid.Parse(message)).Result.ItemSerials;

            foreach (var serial in serials)
            {
                items.Add(new Items() { Text = serial.Name + " (" + serial.Year + ")", Data = "s/" + serial.Id });
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
        else if (type == "rf") //TODO: проверка на большом количестве 
        {
            var films = dataManager.itemFilm.GetItemFilms().Result.ToList();

            films.Sort((x, y) => ((double.Parse(y.RatingKP) * double.Parse(y.VotesKP) + double.Parse(y.RatingIMDB) * double.Parse(y.VotesIMDB)) / 2)
                                .CompareTo((double.Parse(x.RatingKP) * double.Parse(x.VotesKP) + double.Parse(x.RatingIMDB) * double.Parse(x.VotesIMDB)) / 2));

            foreach (var film in films)
            {
                items.Add(new Items() { Text = film.Name + " (" + film.Year + ")", Data = "m/" + film.Id });
            }

            return ViewItems(items, message, type, page);
        }
        else if (type == "rs") //TODO: проверка на большом количестве 
        {
            var serials = dataManager.itemSerials.GetItemSerials().Result.ToList();

            serials.Sort((x, y) => ((double.Parse(y.RatingKP) * double.Parse(y.VotesKP) + double.Parse(y.RatingIMDB) * double.Parse(y.VotesIMDB)) / 2)
                                .CompareTo((double.Parse(x.RatingKP) * double.Parse(x.VotesKP) + double.Parse(x.RatingIMDB) * double.Parse(x.VotesIMDB)) / 2));

            foreach (var serial in serials)
            {
                items.Add(new Items() { Text = serial.Name + " (" + serial.Year + ")", Data = "s/" + serial.Id + "/" + type + "/" + page });
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
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("-->", "move/" + type + "/" + message + "/" + (page + 1)) });
        }
        else if (items.Count - page * 10 < 10 && items.Count > 10)
        {
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", "move/" + type + "/" + message + "/" + (page - 1)) });
        }
        else if (items.Count > 10 && page > 1)
        {
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("<--", "move/" + type + "/" + message + "/" + (page - 1)),
                                    InlineKeyboardButton.WithCallbackData("-->", "move/" + type + "/" + message + "/" + (page + 1))});
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
