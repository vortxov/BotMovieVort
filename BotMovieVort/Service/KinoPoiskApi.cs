using BotMovieVort.Domain.Entity;
using BotMovieVort.Model;
using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BotMovieVort.Service
{
    public class KinoPoiskApi
    {
        public ItemFilm GetRatingFilm(ItemFilm itemFilm)
        {
           var docs = GetRequest(itemFilm.Name);
           foreach (var item in docs.docs)
           {
               if(item.name == itemFilm.Name && item.year.ToString() == itemFilm.Year)
               {
                    itemFilm.RatingKP = item.rating.kp.ToString();
                    itemFilm.RatingIMDB = item.rating.imdb.ToString();
                    itemFilm.VotesIMDB = item.votes.imdb.ToString();
                    itemFilm.VotesKP = item.votes.kp.ToString();
                    break;
               }
           }
           return itemFilm;
        }

        public ItemSerials GetRatingSerial(ItemSerials itemSerials)
        {
            var docs = GetRequest(itemSerials.Name);
            foreach (var item in docs.docs)
            {
                if (item.name == itemSerials.Name && item.year.ToString() == itemSerials.Year)
                {
                    itemSerials.RatingKP = item.rating.kp.ToString();
                    itemSerials.RatingIMDB = item.rating.imdb.ToString();
                    itemSerials.VotesIMDB = item.votes.imdb.ToString();
                    itemSerials.VotesKP = item.votes.kp.ToString();
                    break;
                }
            }
            return itemSerials;
        }

        public Docs GetRequest(string name) //TODO: токен и тд
        {
            var baseAddress = new Uri("https://api.kinopoisk.dev");
            using (var response = new HttpRequest())
            {
                var res = response.Get("https://api.kinopoisk.dev/movie?search=" + name + "&field=name&token=ZQQ8GMN-TN54SGK-NB3MKEC-ZKB8V06&isStrict=false").ToString();

                return JsonSerializer.Deserialize<Docs>(res);
            }
        }
    }
}
