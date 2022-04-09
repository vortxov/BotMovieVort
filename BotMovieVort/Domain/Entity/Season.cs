using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Entity
{
    public class Season
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public List<Series> Series { get; set; } = new List<Series>();
        public ItemSerials Serials { get; set; }
    }
}
