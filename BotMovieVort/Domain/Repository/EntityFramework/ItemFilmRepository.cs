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
    public class ItemFilmRepository : IItemFilmRepository
    {
        AppDbContext appDbContext;
        public ItemFilmRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task DeleteItemFilm(Guid id)
        {
            appDbContext.ItemFilms.Remove(GetItemFilmById(id).Result);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<ItemFilm> GetItemFilmById(Guid id)
        {
            return appDbContext.ItemFilms.Include(x => x.Genres).FirstOrDefault(x => x.Id == id);
        }

        public async Task<ItemFilm> GetItemFilmByNameAndYear(string name, string year)
        {
            return appDbContext.ItemFilms.Include(x => x.Genres).FirstOrDefault(x => x.Name == name && x.Year == year);
        }

        public async Task<IQueryable<ItemFilm>> GetItemFilms()
        {
            return appDbContext.ItemFilms.Include(x => x.Genres);
        }

        public async Task SaveItemFilm(ItemFilm entity)
        {
            if (GetItemFilmById(entity.Id).Result == null)
            {
                await appDbContext.ItemFilms.AddAsync(entity);
                await appDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateItemFilm(ItemFilm entity)
        {
            if (GetItemFilmById(entity.Id).Result != null)
            {
                appDbContext.ItemFilms.Update(entity);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
