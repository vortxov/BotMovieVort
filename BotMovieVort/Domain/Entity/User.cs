using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public long UserTelegramId { get; set; }
        public List<ItemSerials> ItemSerials { get; set; } = new List<ItemSerials>();
    }
}
