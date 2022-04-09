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
    public class SeriesRepository : ISeriesRepository
    {
        AppDbContext appDbContext;
        public SeriesRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task DeleteSeries(Guid id)
        {
            appDbContext.Series.Remove(GetSeriesById(id).Result);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<Series> GetSeriesById(Guid id)
        {
            return appDbContext.Series.Include(x => x.Season).ThenInclude(x => x.Serials).Include(x => x.Season).ThenInclude(x => x.Series).FirstOrDefault(x => x.Id == id);
        }

        public async Task<IQueryable<Series>> GetSeries()
        {
            return appDbContext.Series.Include(x => x.Season);
        }

        public async Task SaveSeries(Series entity)
        {
            if (GetSeriesById(entity.Id).Result == null)
            {
                await appDbContext.Series.AddAsync(entity);
                await appDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateSeries(Series entity)
        {
            if (GetSeriesById(entity.Id).Result != null)
            {
                appDbContext.Series.Update(entity);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
