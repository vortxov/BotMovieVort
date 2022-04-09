using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Entity
{
    public class Series
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string Path { get; set; }
        public int NumberSeries { get; set; }
        public string? FileId { get; set; }
        public Season Season { get; set; }
    }
}
