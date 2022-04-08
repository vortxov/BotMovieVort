using BotMovieVort.Domain.Repository.Abstract;
using BotMovieVort.Domain.Repository.EntityFramework;

namespace BotMovieVort.Domain
{
    public class DataManager
    {
        public AppDbContext appDbContext { get; set; }
        public IGenreRepository genre { get; set; }
        public IItemFilmRepository itemFilm { get; set; }
        public IItemSerialsRepository itemSerials { get; set; }
        public ISeasonRepository season { get; set; }
        public ISeriesRepository series { get; set; }
        public DataManager()
        {
            appDbContext = new AppDbContext();
            genre = new GenreRepository(appDbContext);
            itemSerials = new ItemSerialsRepository(appDbContext);
            itemFilm = new ItemFilmRepository(appDbContext);
            series = new SeriesRepository(appDbContext);
            season = new SeasonRepository(appDbContext);
        }
    }
}
