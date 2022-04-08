using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Entity
{
    public class ItemSerials
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public string Description { get; set; }
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<Season> Seasons { get; set; } = new List<Season>();
        public string? RatingKP { get; set; }
        public string? RatingIMDB { get; set; }
        public string? VotesKP { get; set; }
        public string? VotesIMDB { get; set; }
        public string? Status { get; set; }
    }
}
