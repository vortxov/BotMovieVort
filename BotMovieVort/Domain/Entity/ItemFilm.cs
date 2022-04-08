using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Entity
{
    public class ItemFilm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Year { get; set; }
        public List<Genre> Genres { get; set; }
        public string Description { get; set; }
        public string? RatingKP { get; set; }
        public string? RatingIMDB { get; set; }
        public string? VotesKP { get; set; }
        public string? VotesIMDB { get; set; }
        public string? FileId { get; set; }
    } 
}
