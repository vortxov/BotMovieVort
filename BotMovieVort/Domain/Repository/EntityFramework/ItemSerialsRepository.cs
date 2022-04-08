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
    public class ItemSerialsRepository : IItemSerialsRepository
    {
        AppDbContext appDbContext;
        public ItemSerialsRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task DeleteItemSerials(Guid id)
        {
            appDbContext.ItemSerials.Remove(GetItemSerialsById(id).Result);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<ItemSerials> GetItemSerialsById(Guid id)
        {
            return appDbContext.ItemSerials.Include(x => x.Seasons).ThenInclude(x => x.Series).Include(x => x.Genres).FirstOrDefault(x => x.Id == id);
        }

        public async Task<ItemSerials> GetItemSerialsByNameAndYear(string name, string year)
        {
            return appDbContext.ItemSerials.Include(x => x.Seasons).ThenInclude(x => x.Series).Include(x => x.Genres).FirstOrDefault(x => x.Name == name && x.Year == year);
        }

        public async Task<IQueryable<ItemSerials>> GetItemSerials()
        {
            return appDbContext.ItemSerials.Include(x => x.Seasons).ThenInclude(x => x.Series).Include(x => x.Genres);
        }

        public async Task SaveItemSerials(ItemSerials entity)
        {
            if (GetItemSerialsById(entity.Id).Result == null)
            {
                await appDbContext.ItemSerials.AddAsync(entity);
                await appDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateItemSerials(ItemSerials entity)
        {
            if (GetItemSerialsById(entity.Id).Result != null)
            {
                appDbContext.ItemSerials.Update(entity);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
