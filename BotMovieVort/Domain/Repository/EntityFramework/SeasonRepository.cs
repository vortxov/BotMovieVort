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
    public class SeasonRepository : ISeasonRepository
    {
        AppDbContext appDbContext;
        public SeasonRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task DeleteSeason(Guid id)
        {
            appDbContext.Seasons.Remove(GetSeasonById(id).Result);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<Season> GetSeasonById(Guid id)
        {
            return appDbContext.Seasons.Include(x => x.Series).Include(x => x.Serials).FirstOrDefault(x => x.Id == id);
        }

        public async Task<IQueryable<Season>> GetSeasons()
        {
            return appDbContext.Seasons.Include(x => x.Series).Include(x => x.Serials);
        }

        public async Task SaveSeason(Season entity)
        {
            if (GetSeasonById(entity.Id).Result == null)
            {
                await appDbContext.Seasons.AddAsync(entity);
                await appDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateSeason(Season entity)
        {
            if (GetSeasonById(entity.Id).Result != null)
            {
                appDbContext.Seasons.Update(entity);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
