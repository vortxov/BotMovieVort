using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Entity
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ItemFilm> ItemFilms { get; set; }
        public List<ItemSerials> ItemSerials { get; set; }
    }
}
