using BotMovieVort.Domain.Entity;
using BotMovieVort.Domain.Repository.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Repository.EntityFramework
{
    public class GenreRepository : IGenreRepository
    {
        AppDbContext appDbContext;
        public GenreRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task DeleteGenre(Guid id)
        {
            appDbContext.Genres.Remove(GetGenreById(id).Result);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<Genre> GetGenreById(Guid id)
        {
            return appDbContext.Genres.Include(x => x.ItemFilms).Include(x => x.ItemSerials).FirstOrDefault(x => x.Id == id);
        }

        public async Task<Genre> GetGenreByName(string name)
        {
            return appDbContext.Genres.Include(x => x.ItemFilms).Include(x => x.ItemSerials).FirstOrDefault(x => x.Name == name);
        }

        public async Task<IQueryable<Genre>> GetGenres()
        {
            return appDbContext.Genres.Include(x => x.ItemFilms).Include(x => x.ItemSerials);
        }

        public async Task SaveGenre(Genre entity)
        {
            if (GetGenreById(entity.Id).Result == null)
            {
                await appDbContext.Genres.AddAsync(entity);
                await appDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateGenre(Genre entity)
        {
            if (GetGenreById(entity.Id).Result != null)
            {
                appDbContext.Genres.Update(entity);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
