using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Model
{
    public class Movie
    {
        public int id { get; set; }
        public string name { get; set; }
        public string enName { get; set; }
        public string type { get; set; }
        public int year { get; set; }
        public string status { get; set; }
        public Rating rating { get; set; }
        public Rating votes { get; set; }
    }
}
