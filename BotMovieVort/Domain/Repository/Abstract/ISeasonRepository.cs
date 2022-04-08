using BotMovieVort.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Repository.Abstract
{
    public interface ISeasonRepository
    {
        Task<IQueryable<Season>> GetSeasons();
        Task<Season> GetSeasonById(Guid id);
        Task SaveSeason(Season entity);
        Task DeleteSeason(Guid id);
        Task UpdateSeason(Season entity);
    }
}
