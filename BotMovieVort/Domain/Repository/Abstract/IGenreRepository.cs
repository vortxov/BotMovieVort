using BotMovieVort.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMovieVort.Domain.Repository.Abstract
{
    public interface IGenreRepository
    {
        Task<IQueryable<Genre>> GetGenres();
        Task<Genre> GetGenreById(Guid id);
        Task SaveGenre(Genre entity);
        Task DeleteGenre(Guid id);
        Task UpdateGenre(Genre entity);
        Task<Genre> GetGenreByName(string name);
    }
}
