using BotMovieVort.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Repository.Abstract
{
    public interface ISeriesRepository
    {
        Task<IQueryable<Series>> GetSeries();
        Task<Series> GetSeriesById(Guid id);
        Task SaveSeries(Series entity);
        Task DeleteSeries(Guid id);
        Task UpdateSeries(Series entity);
    }
}
