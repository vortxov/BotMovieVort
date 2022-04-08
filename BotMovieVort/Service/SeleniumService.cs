using BotMovieVort.Domain;
using BotMovieVort.Domain.Entity;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Service
{
    public class SeleniumService
    {
        private const string BaseAddress = "https://aptracker.ru/";
        private const string FilmAddress = "load/mp4_films/86-";
        private const string SerialyAddress = "load/mp4_serialy/87-";
        private const string PathTorrent = @"C:\Vort\td\Bot\BotMovieVort\BotMovieVort\Torrent\a.torrent";
        IWebDriver driver;
        private DataManager dataManager;
        private KinoPoiskApi kinoPoiskApi;
        private TdApiServer tdApi;

        public SeleniumService() //TODO: Последние обновлнение на сайте
        {
            dataManager = new DataManager();
            kinoPoiskApi = new KinoPoiskApi();
            tdApi = new TdApiServer();
            TdApiServer.Start();
        }

        public async void ContentFilm()
        {
            int p = 1;

            driver = new ChromeDriver("C:\\Vort");
            driver.Manage().Window.Maximize();

            while (true)
            {
                try
                {
                    driver.Navigate().GoToUrl(BaseAddress + FilmAddress + p.ToString());
                    //System.Threading.Thread.Sleep(1000);

                    var listEntry = driver.FindElements(By.Id("mp4"));

                    List<string> urlFilms = new List<string>();

                    foreach (var entry in listEntry)
                    {
                        try
                        {
                            var close = entry.FindElement(By.ClassName("f-size"));
                            if (close.Text == "ЗАКРЫТ")
                                continue;
                            var hrefFilm = entry.FindElement(By.ClassName("head-title-s"));

                            urlFilms.Add(hrefFilm.GetAttribute("href"));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }

                    foreach (var urlFilm in urlFilms)
                    {
                        try
                        {
                            driver.Navigate().GoToUrl(urlFilm);
                            var down = driver.FindElement(By.ClassName("dowmn_l"));
                            var url = down.FindElement(By.TagName("a")).GetAttribute("href");

                            var film = FilmPage();

                            var filmDb = dataManager.itemFilm.GetItemFilmByNameAndYear(film.Name, film.Year).Result;

                            if (filmDb == null)
                            {

                                using (var client = new WebClient())
                                {
                                    client.DownloadFile(url, PathTorrent);
                                }

                                var listPath = UTorrentService.DownloadTorrent(PathTorrent);
                                File.Delete(PathTorrent);
                                if (listPath == null)
                                    continue;

                                if (listPath.Count == 1) //TODO: множество файлов при скачивании фильма
                                {
                                    film.Path = listPath[0];
                                }


                                film = kinoPoiskApi.GetRatingFilm(film);
                                film.Id = Guid.NewGuid();

                                await dataManager.itemFilm.SaveItemFilm(film);

                                TdApiServer.getFileId(film.Path, "m", film.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                    p++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }


        public async void ContentSerial()
        {
            int p = 1;

            driver = new ChromeDriver("C:\\Vort");
            driver.Manage().Window.Maximize();

            while (true)
            {
                try
                {
                    driver.Navigate().GoToUrl(BaseAddress + SerialyAddress + p.ToString());
                    //System.Threading.Thread.Sleep(1000);

                    var listEntry = driver.FindElements(By.Id("mp4"));

                    List<string> urlFilms = new List<string>();

                    foreach (var entry in listEntry)
                    {
                        try
                        {
                            var close = entry.FindElement(By.ClassName("f-size"));
                            if (close.Text == "ЗАКРЫТ")
                                continue;
                            var hrefFilm = entry.FindElement(By.ClassName("head-title-s"));

                            urlFilms.Add(hrefFilm.GetAttribute("href"));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }

                    foreach (var urlFilm in urlFilms)
                    {
                        try
                        {
                            driver.Navigate().GoToUrl(urlFilm);
                            var down = driver.FindElement(By.ClassName("dowmn_l"));
                            var url = down.FindElement(By.TagName("a")).GetAttribute("href");

                            var serial = SerialPage();

                            using (var client = new WebClient())
                            {
                                client.DownloadFile(url, PathTorrent);
                            }

                            var listPath = UTorrentService.DownloadTorrent(PathTorrent);
                            File.Delete(PathTorrent);
                            if (listPath == null)
                                continue;

                            var serialDb = dataManager.itemSerials.GetItemSerialsByNameAndYear(serial.Name, serial.Year).Result;


                            if (serialDb != null)
                            {
                                serial = serialDb;

                                foreach (var path in listPath)
                                {
                                    var nameSeries = path.Split(@"\").Last();
                                    var infoSeries = nameSeries.Split(".s")[1].Split(".")[0].Split("e");

                                    var season = new Season() { Number = int.Parse(infoSeries[0]) };

                                    if(serial.Seasons.FirstOrDefault(x => x.Number == season.Number) == null)
                                    {
                                        serial.Seasons.Add(season);
                                    }
                                    season = serial.Seasons.FirstOrDefault(x => x.Number == season.Number);

                                    var series = new Series() {Id = Guid.NewGuid(), NumberSeries = int.Parse(infoSeries[1]), Path = path }; //TODO: Гдето найти имя серии и вставлять

                                    if(season.Series.FirstOrDefault(x => x.NumberSeries == series.NumberSeries) == null)
                                    {
                                        season.Series.Add(series);
                                    }
                                }


                                dataManager.itemSerials.UpdateItemSerials(serial);

                                foreach (var season in serial.Seasons)
                                {
                                    foreach (var item in season.Series)
                                    {
                                        if(item.FileId == null)
                                        {
                                            TdApiServer.getFileId(item.Path, "s", item.Id);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                foreach (var path in listPath)
                                {
                                    var nameSeries = path.Split(@"\").Last();
                                    var infoSeries = nameSeries.Split(".s")[1].Split(".")[0].Split("e");

                                    var season = new Season() { Number = int.Parse(infoSeries[0]) };

                                    if (serial.Seasons.FirstOrDefault(x => x.Number == season.Number) == null)
                                    {
                                        serial.Seasons.Add(season);
                                    }
                                    season = serial.Seasons.FirstOrDefault(x => x.Number == season.Number);

                                    var series = new Series() { NumberSeries = int.Parse(infoSeries[1]), Path = path }; //TODO: Гдето найти имя серии и вставлять
                                    season.Series.Add(series);
                                }


                                serial = kinoPoiskApi.GetRatingSerial(serial);
                                dataManager.itemSerials.SaveItemSerials(serial);

                                foreach (var season in serial.Seasons)
                                {
                                    foreach (var item in season.Series)
                                    {
                                        if (item.FileId == null)
                                        {
                                            TdApiServer.getFileId(item.Path, "s", item.Id);
                                        }
                                    }
                                }
                            }

                            


                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                    p++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }


        public ItemFilm FilmPage()
        {
            var headTitle = driver.FindElement(By.ClassName("head-title")).Text;
            var eText = driver.FindElement(By.ClassName("eText"));
            var text = eText.FindElements(By.TagName("p"))[0].Text;
            var listText = text.Split("\r\n");
            var listGenre = listText[1].Split(": ")[1].Split(", ");

            List<Genre> genres = new List<Genre>();
            foreach (var genre in listGenre)
            {
                var gerreDb = dataManager.genre.GetGenreByName(genre).Result;
                if (gerreDb != null)
                    genres.Add(gerreDb);
                
                else
                    genres.Add(new Genre() { Name = genre });
            }

            var film = new ItemFilm();
            film.Name = headTitle.Split(" (")[0].Split(" [")[0];
            film.Year = listText[0].Split(": ")[1].Split("-")[0]; 
            film.Genres = genres;
            film.Description = listText[9]; //TODO: Добавить список актеров для поиска по актерам 
           

            return film;
        }


        public ItemSerials SerialPage()
        {
            var headTitle = driver.FindElement(By.ClassName("head-title")).Text;
            var eText = driver.FindElement(By.ClassName("eText"));
            var text = eText.FindElements(By.TagName("p"))[0].Text;
            var listText = text.Split("\r\n");
            var listGenre = listText[1].Split(": ")[1].Split(", ");

            List<Genre> genres = new List<Genre>();
            foreach (var genre in listGenre)
            {
                var gerreDb = dataManager.genre.GetGenreByName(genre).Result;
                if (gerreDb != null)
                    genres.Add(gerreDb);

                else
                    genres.Add(new Genre() { Name = genre });
            }

            var serial = new ItemSerials();
            serial.Name = headTitle.Split(" (")[0].Split(" [")[0];
            serial.Year = listText[0].Split(": ")[1].Split("-")[0];
            serial.Genres = genres;
            serial.Description = listText[9]; //TODO: Добавить список актеров для поиска по актерам 


            return serial;
        }
    }
}
